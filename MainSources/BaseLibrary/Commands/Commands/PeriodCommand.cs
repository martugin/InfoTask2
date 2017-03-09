using System;

namespace BaseLibrary
{
    //Команда, задающая период обработки
    public class PeriodCommand : Command
    {
        public PeriodCommand(Logger logger, Command parent, DateTime begin, DateTime end, string mode) 
            : base(logger, parent, 0 , 100)
        {
            Begin = begin;
            End = end;
            Mode = mode;
        }

        //Начало, конец и режим периода
        public DateTime Begin { get; private set; }
        public DateTime End { get; private set; }
        public string Mode { get; private set; }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            Logger.PeriodCommand = null;
        }
    }
}