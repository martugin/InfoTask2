using System;

namespace BaseLibrary
{
    //Логгер с понятием период обработки
    public abstract class LoggerTimed : Logg
    {


        //Начало и конец текущего периода обработки
        public DateTime BeginPeriod { get; protected set; }
        public DateTime EndPeriod { get; protected set; }
        //Режим (Выравнивание, Синхроннный, Разовый и т.п.)
        public string ModePeriod { get; protected set; }
    }
}