
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
            get { lock (_procentLock) return _procent; }
            set
            {
                lock (_procentLock)
                {
                    _procent = value;
                    if (Parent != null)
                        Parent.Procent = _startProcent + value * (_finishProcent - _startProcent) / 100;
                }
            }
        }
        //Каким значениям родителя соответствует 0% и 100% счетчика процентов
        private readonly double _startProcent;
        private readonly double _finishProcent;

        //Объекты для блокировки качества и процентов потоком
        private readonly object _qualityLock = new object();
        private readonly object _procentLock = new object();

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
        public virtual Comm Run(Action action)
        {
            action();
            return Finish();
        }

        //Завершает комманду вместе со всеми детьми
        public Comm Finish(bool isBreaked = false)
        {
            var c = Logger.Command;
            while (c != this)
            {
                c.FinishCommand(isBreaked);
                c = c.Parent;
            }
            FinishCommand(isBreaked);
            return this;
        }

        //Завершает комманду и ее же возвращает
        protected virtual void FinishCommand(bool isBreaked) //Комманда была прервана
        {
            Procent = 100;
            IsFinished = true;
            IsBreaked = isBreaked;
            Logger.Command = Logger.Command.Parent;
        }
    }
}