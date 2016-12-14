using System;

namespace BaseLibrary
{
    //Логгер с понятием период обработки
    public abstract class LoggerTimed : Logg
    {
        protected LoggerTimed(IHistory history) 
            : base(history) { }
        
        //Начало и конец текущего периода обработки
        private DateTime _beginPeriod;
        
        public DateTime BeginPeriod
        {
            get { lock (_timeLocker) return _beginPeriod; }
            protected set
            {
                lock (_timeLocker)
                    _beginPeriod = value;
                //ToDo событие
            }
        }
        private DateTime _endPeriod;
        public DateTime EndPeriod
        {
            get { lock (_timeLocker) return _endPeriod; }
            protected set
            {
                lock (_timeLocker)
                    _endPeriod = value;
                //ToDo событие
            }
        }
        private readonly object _timeLocker = new object();

        //Режим (Выравнивание, Синхроннный, Разовый и т.п.)
        public string ModePeriod { get; protected set; }
    }
}