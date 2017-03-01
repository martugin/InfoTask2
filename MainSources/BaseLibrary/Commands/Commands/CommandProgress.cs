using System;

namespace BaseLibrary
{
    //Команда для отображения индикатора и записи в SuperHistory
    public class CommandProgress : CommandLogBase
    {
        //Конструктор с указанием периода обработки
        protected internal CommandProgress(Logger logger, Command parent, DateTime begin, DateTime end, string mode, string name, string pars, DateTime? endTime)
            : base(logger, parent, 0, 100, name, pars)
        {
            Logger.CallShowIndicatorTimed();
            BeginPeriod = begin;
            EndPeriod = end;
            ModePeriod = mode;
            Logger.SetPeriod(begin, end, mode);
            Initialize(endTime);
        }
        //Конструктор с указанием текста 0-го уровня формы индикатора
        protected internal CommandProgress(Logger logger, Command parent, string text, string name, string pars, DateTime? endTime)
            : base(logger, parent, 0, 100, name, pars)
        {
            Logger.CallShowIndicatorTexted();
            Logger.SetTabloText(0, text);
            Initialize(endTime);
        }
        private void Initialize(DateTime? endTime)//Если не null, то время конца обратного отсчета
        {
            if (endTime != null)
                Logger.CallSetProcentTimed((DateTime) endTime);
            else Logger.CallSetProcentUsual();
            Logger.TabloProcent = 0;
            if (History != null)
                History.WriteStartSuper(this);
        }
        
        //Период обработки
        public DateTime BeginPeriod { get; private set; }
        public DateTime EndPeriod { get; private set; }
        //Режим выполнения
        public string ModePeriod { get; private set; }

        //Отобразить индикатор
        public override double Procent
        {
            get { return Logger.TabloProcent; }
            set { Logger.TabloProcent = value; }
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null)
                History.WriteFinishSuper(this, null);
            Logger.SetTabloText(0, "");
            Logger.CallHideIndicator();
            Logger.CommandProgress = null;
        }
    }
}