using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLibrary
{
    //Предок для всех типов комманд
    public class Command : IDisposable
    {
        //Создание новой комманды 
        internal Command(Logger logger, //логгер
                                  Command parent, //команда содержащая данную
                                  double start, //проценты от родителя
                                  double finish,
                                  CommandFlags flags, //флаги команд Progress, Collect, Danger
                                  string context) //контекст для записи в лог
        {
            Logger = logger;
            Parent = parent;
            _startProcent = start;
            _finishProcent = finish;
            IsProgress = flags.HasFlag(CommandFlags.Progress);
            IsCollect = flags.HasFlag(CommandFlags.Collect); 
            IsDanger = flags.HasFlag(CommandFlags.Danger);
            Context = !Context.IsEmpty() ? context : Parent.Context;
            Procent = 0;
        }

        //Создание новой комманды без указания процентов
        internal Command(Logger logger, Command parent, CommandFlags flags, string context)
            : this(logger, parent, parent == null ? 0 : parent.Procent, parent == null ? 100 : parent.Procent, flags, context)
        {
        }

        //Указатель на родителя
        protected Command Parent { get; private set; }
        //Указатель на Logger
        protected Logger Logger { get; private set; }
        //Контекст выполнения 
        internal string Context { get; private set; }

        //Признаки разных типов комманды
        internal protected bool IsProgress { get; private set; }
        internal protected bool IsCollect { get; private set; }
        internal protected bool IsDanger { get; private set; }

        //Добавить в комманду ошибку error, isRepeated - ошибка произошла в повторяемой операции
        public virtual void AddError(ErrorCommand error, bool isRepeated)
        {
            ChangeQuality(error, isRepeated);
            if (Parent != null) 
                Parent.AddError(error, isRepeated || IsDanger);
            if (IsCollect && !isRepeated)
            {
                if (_errors == null) _errors = new List<ErrorCommand>();
                bool isFound = false;
                foreach (var err in _errors)
                    if (err.EqualsTo(error))
                        isFound = true;
                if (!isFound) _errors.Add(error);
            }
        }

        //Изменить ошибочнось комманды
        protected void ChangeQuality(ErrorCommand error, bool isRepeated)
        {
            CommandQuality q = error.Quality;
            if (isRepeated && q == CommandQuality.Error) q = CommandQuality.Repeat;
            if (Quality < q) Quality = q;
        }

        //Ошибочность комманды
        private CommandQuality _quality = CommandQuality.Success;
        public CommandQuality Quality
        {
            get { lock (_errorLock) return _quality;}
            private set { lock(_errorLock ) _quality = value; }
        }
        //True, если при выполнении комманды произошла ошибка
        public bool IsError { get { return Quality == CommandQuality.Error; } }

        //Список ошибок 
        private List<ErrorCommand> _errors;

        //Совокупное сообщение об ошибках
        //Добавляются в описание: addContext - контекст ошибки, addParams - параметры, addErrType - добавлять подпись Ошибка или Предупреждение
        public string ErrorMessage(bool addContext = true, bool addParams = true, bool addErrType = true)
        {
            if (_errors == null || _errors.Count == 0) return "";

            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var e in _errors.Where(e => e.Quality == CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType) sb.Append("Ошибка: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty())
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty())
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            foreach (var e in _errors.Where(e => e.Quality != CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType) sb.Append("Предупреждение: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty())
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty())
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            return sb.ToString();
        }

        //Объекты для блокировки ошибок и процентов потоком
        private readonly object _errorLock = new object();
        private readonly object _procentLock = new object();

        //Величина
        private double _procent;
        public virtual double Procent
        {
            get { lock (_procentLock) return _procent; }
            set
            {
                lock (_procentLock)
                {
                    _procent = value;
                    if (IsProgress)
                        Logger.ViewProcent(value);
                    else if (Parent != null)
                        Parent.Procent = _startProcent + value * (_finishProcent - _startProcent) / 100;
                }
            }
        }
        //Каким значениям родителя соответствует 0% и 100% счетчика
        private readonly double _startProcent;
        private readonly double _finishProcent;

        //Команда завершена
        public bool IsFinished { get; private set; }
        //Комманда была прервана
        public bool IsBreaked { protected get; set; }

        //Завершает комманду и ее же возвращает
        public Command Finish(string results = null, //Результаты для записи в лог или отображения
                                          bool isBreaked = false) //Комманда была прервана
        {
            Procent = 100;
            IsFinished = true;
            IsBreaked = isBreaked;
            Logger.Command = Parent;
            FinishCommand(results);
            return this;
        }

        //Действия по завершению комманды, индывидуальные для разных типов комманд
        protected virtual void FinishCommand(string results)
        {
            if (IsProgress) 
                Logger.FinishProgressCommand();
        }

        //Очистка ресурсов
        public void Dispose()
        {
            try 
            { 
                while (Logger.Command != this && Logger.Command != null)
                    Logger.Finish(); 
            }
            catch { }
        }
    }
}
