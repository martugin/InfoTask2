using System;

namespace BaseLibrary
{
    //Команда для отображения индикатора и записи в SuperHistory
    public class ProgressCommand : LogBaseCommand
    {
        //Конструктор с указанием периода обработки
        protected internal ProgressCommand(Logger logger, Command parent, DateTime begin, DateTime end, string mode, string name, string pars, DateTime? endTime)
            : base(logger, parent, 0, 100, name, pars)
        {
            Logger.PeriodBegin = PeriodBegin = begin;
            Logger.PeriodEnd = PeriodEnd = end;
            Logger.PeriodMode = PeriodMode = mode;
            if (Indicator != null)
            {
                Indicator.ShowTimedIndicator();
                Indicator.ChangePeriod(begin, end, mode);
            }
            Initialize(endTime);
        }
        //Конструктор с указанием текста 0-го уровня формы индикатора
        protected internal ProgressCommand(Logger logger, Command parent, string text, string name, string pars, DateTime? endTime)
            : base(logger, parent, 0, 100, name, pars)
        {
            Indicator.ShowTextedIndicator();
            Logger.SetTabloText(0, text);
            Initialize(endTime);
        }
        private void Initialize(DateTime? endTime)//Если не null, то время конца обратного отсчета
        {
            if (Indicator != null)
            {
                if (endTime != null)
                    Indicator.SetProcessTimed((DateTime)endTime);
                else Indicator.SetProcessUsual();
                Indicator.ChangeProcent(0);    
            }
            if (History != null)
                History.WriteStartSuper(this);
        }

        //Ссылка на индикатор
        private IIndicator Indicator {get { return Logger.Indicator; }}

        //Период обработки
        public DateTime PeriodBegin { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        //Режим выполнения
        public string PeriodMode { get; private set; }

        //Отобразить индикатор
        public override double Procent
        {
            get { return base.Procent; }
            set
            {
                base.Procent = value; 
                if (Indicator != null)
                    Indicator.ChangeProcent(value);
            }
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null)
                History.WriteFinishSuper(this, null);
            Logger.SetTabloText(0, "");
            if (Indicator != null)
                Indicator.HideIndicator();
            Logger.ProgressCommand = null;
        }
    }
}