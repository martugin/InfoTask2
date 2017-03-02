using System;

namespace BaseLibrary
{
    //Простая комманда
    public class Command : IDisposable
    {
        //Создание новой комманды 
        internal Command(Logger logger, Command parent, double startProcent, double finishProcent)
        {
            Logger = logger;
            Parent = parent;
            _startProcent = startProcent;
            _finishProcent = finishProcent;
            Procent = 0;
        }

        //Указатель на родителя
        protected internal Command Parent { get; private set; }
        //Указатель на Logger
        protected internal Logger Logger { get; private set; }

        //Качество команды
        private CommandQuality _quality = CommandQuality.Success;
        public CommandQuality Quality { get { lock (_qualityLock) return _quality; } }
        //Команда с ошибкой
        public bool IsError { get { return Quality == CommandQuality.Error; } }
        //Команда совсем без ошибки
        public bool IsSuccess { get { return Quality == CommandQuality.Success; } }

        //Добавить ошибку
        public virtual void AddError(CommandError err)
        {
            AddQuality(err.Quality);
            if (Parent != null)
                Parent.AddError(err);
        }
        protected void AddQuality(CommandQuality quality)
        {
            lock (_qualityLock)
                if (_quality < quality)
                    _quality = quality;
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
        protected internal string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                return Quality.ToRussian();
            }
        }

        //Запуск операции, обрамляемой данной командой
        public virtual Command Run(Action action) //Возвращает описание результатов операции
        {
            if (IsFinished) return this;
            action();
            return Finish();
        }
        
        //Завершить команду
        public Command Finish()
        {
            while (Logger.Command != this)
                Logger.Command.FinishCommand(false);
            FinishCommand(false);
            Logger.CheckBreak();
            return this;
        }

        //Завершает комманду
        protected internal virtual void FinishCommand(bool isBreaked) //Комманда была прервана
        {
            Procent = 100;
            IsFinished = true;
            IsBreaked = isBreaked;
            Logger.Command = Logger.Command.Parent;
        }

        //Очистка ресурсов
        public void Dispose()
        {
            try { Finish(); }
            catch {}
        }
    }
}