﻿using System;

namespace BaseLibrary
{
    //Логгер с понятием период обработки
    public abstract class LoggerTimed : Logg
    {
        protected LoggerTimed(LoggerDangerness dangerness) 
            : base(dangerness) { }
        
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

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public CommProgress StartProgress(DateTime begin, DateTime end, string mode, string name, string pars = "")
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommProgress(this, Command, begin, end, mode, name, pars);
            return CommandProgress;
        }
    }
}