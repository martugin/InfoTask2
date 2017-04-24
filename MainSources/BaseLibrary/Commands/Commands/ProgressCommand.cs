using System;

namespace BaseLibrary
{
    //Команда для отображения индикатора и записи в SuperHistory
    public class ProgressCommand : LogBaseCommand
    {
        protected internal ProgressCommand(Logger logger, Command parent, string name, string pars, string context, DateTime? endTime) //Время окончания индикатора для отсчета времени
            : base(logger, parent, 0, 100, name, pars, context)
        {
            if (Indicator != null)
            {
                if (Logger.PeriodCommand != null)
                {
                    Indicator.ShowTimedIndicator();
                    Indicator.ChangePeriod(Logger.PeriodBegin, Logger.PeriodEnd, Logger.PeriodMode);
                }
                else Indicator.ShowTextedIndicator();
                Logger.SetTabloText(0, name);
                if (endTime != null)
                    Indicator.SetTimedProcess((DateTime)endTime);
                else Indicator.SetProcessUsual();
                Indicator.ChangeProcent(0);    
            }
            if (History != null)
                History.WriteStartSuper(this);
        }

        //Ссылка на индикатор
        private IIndicator Indicator {get { return Logger.Indicator; }}

        //Отобразить индикатор
        public override double Procent
        {
            get { return base.Procent; }
            set
            {
                base.Procent = value; 
                if (Indicator != null && Indicator.Procent != Procent)
                    Indicator.ChangeProcent(value);
            }
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null)
                History.WriteFinishSuper(null);
            Logger.SetTabloText(0, "");
            if (Indicator != null)
                Indicator.HideIndicator();
            Logger.ProgressCommand = null;
        }
    }
}