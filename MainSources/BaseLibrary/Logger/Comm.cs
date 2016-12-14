
using System;

namespace BaseLibrary
{
    //Простая комманда
    public class Comm
    {
        //Создание новой комманды 
        internal Comm(Logg logger, Comm parent, double startProcent, double finishProcent)
        {
            Logger = logger;
            Parent = parent;
            _startProcent = startProcent;
            _finishProcent = finishProcent;
        }

        //Указатель на родителя
        internal protected Comm Parent { get; private set; }
        //Указатель на Logger
        internal protected Logg Logger { get; private set; }

        //Качество команды
        private CommandQuality _quality = CommandQuality.Success;
        public CommandQuality Quality { get { lock (_qualityLock) return _quality; }}
        //Команда с ошибкой
        public bool IsError { get { return Quality == CommandQuality.Error; } }
        //Команда совсем без ошибки
        public bool IsSuccess { get { return Quality == CommandQuality.Success; } }

        //Добавить ошибку
        public virtual void AddError(ErrorCommand err)
        {
            lock (_qualityLock)
                if (_quality < err.Quality)
                    _quality = err.Quality;
            if (Parent != null)
                Parent.AddError(err);
        }

        //Относительный процент индикатора
        private double _procent;
        public virtual double Procent
        {
            get { return _procent; }
            set
            {
                _procent = value;
                if (Parent != null)
                    Parent.Procent = _startProcent + value * (_finishProcent - _startProcent) / 100;
            }
        }
        //Каким значениям родителя соответствует 0% и 100% счетчика процентов
        private readonly double _startProcent;
        private readonly double _finishProcent;

        //Объекты для блокировки качества и процентов потоком
        private readonly object _qualityLock = new object();

        //Команда завершена
        public bool IsFinished { get; private set; }
        //Комманда была прервана
        public bool IsBreaked { get; private set; }

        //Строка для записи состояния комманды
        internal protected string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                return Quality.ToRussian();
            }
        }

        //Запуск операции, обрамляемой данной командой
        public virtual Comm Run(Func<string> func) //Возвращает описание результатов операции
        {
            return Finish(func());
        }
        public Comm Run(Action action)
        {
            return Run(() => { action(); return ""; });
        }

        //Завершить команду
        public Comm Finish(string results = "")
        {
            FinishOrBreak(results, false);
        }

        //Прервать команду
        public Comm Break()
        {
            return FinishOrBreak(null, true);
        }

        //Завершает комманду вместе со всеми детьми
        private Comm FinishOrBreak(string results, bool isBreaked)
        {
            var c = Logger.Command;
            while (c != this)
            {
                c.FinishCommand(null, isBreaked);
                c = c.Parent;
            }
            FinishCommand(results, isBreaked);
            return this;
        }

        //Завершает комманду и ее же возвращает
        protected virtual void FinishCommand(string results, bool isBreaked) //Комманда была прервана
        {
            Procent = 100;
            IsFinished = true;
            IsBreaked = isBreaked;
            Logger.Command = Logger.Command.Parent;
        }
    }
}