using System;

namespace BaseLibrary
{
    //Логгер с понятием период обработки
    public abstract class LoggerTimed : Logger
    {
        protected LoggerTimed(LoggerDangerness dangerness) 
            : base(dangerness) { }

        //Событие обновления периода для индикатора
        public event EventHandler<ChangePeriodEventArgs> ChangePeriod;

        //Задать период обработки
        internal protected void SetPeriod(DateTime begin, DateTime end, string mode = "")
        {
            lock (_timeLocker)
            {
                _beginPeriod = begin;
                _endPeriod = end;
                _modePeriod = mode;
                if (ChangePeriod != null) 
                    ChangePeriod(this, new ChangePeriodEventArgs(begin, end, mode));
            }
        }

        //Начало и конец текущего периода обработки
        private DateTime _beginPeriod;
        public DateTime BeginPeriod
        {
            get { lock (_timeLocker) return _beginPeriod; }
        }
        private DateTime _endPeriod;
        public DateTime EndPeriod
        {
            get { lock (_timeLocker) return _endPeriod; }
        }
        
        //Режим (Выравнивание, Синхроннный, Разовый и т.п.)
        private string _modePeriod;
        public string ModePeriod 
        {
            get { lock (_timeLocker) return _modePeriod; } 
        }
        private readonly object _timeLocker = new object();
        
        //Запуск команды логирования в SuperHistory и отображения индикатора
        public CommandProgress StartProgress(DateTime begin, DateTime end, string mode, string name, string pars = "")
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommandProgress(this, Command, begin, end, mode, name, pars);
            return CommandProgress;
        }
    }
}