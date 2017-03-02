using System;
using BaseLibrary;

namespace ComClients
{
    public abstract class IndicatorClient : LoggerClient
    {
        internal IndicatorClient(Logger logger, IIndicator indicator)
        {
            Logger = logger;
            Indicator = indicator;
        }

        //Индикатор
        internal IIndicator Indicator { get; set; }

        //Подписка на события индикатора
        protected void SubscribeEvents()
        {
            Logger.ShowTextedIndicator += Indicator.OnShowTextedIndicator;
            Logger.ShowTimedIndicator += Indicator.OnShowTimedIndicator;
            Logger.HideIndicator += Indicator.OnHideIndicator;
            Logger.ChangeProcent += Indicator.OnChangeProcent;
            Logger.ChangeTabloText += Indicator.OnChangeTabloText;
            Logger.ChangePeriod += Indicator.OnChangePeriod;
            Logger.SetProcentTimed += Indicator.OnSetProcessTimed;
            Logger.SetProcentUsual += Indicator.OnSetProcessUsual;
            Logger.ExecutionFinished += OnExecutionFinished;
        }
        protected void UnsubscribeEvents()
        {
            Logger.ShowTextedIndicator -= Indicator.OnShowTextedIndicator;
            Logger.ShowTimedIndicator -= Indicator.OnShowTimedIndicator;
            Logger.HideIndicator -= Indicator.OnHideIndicator;
            Logger.ChangeProcent -= Indicator.OnChangeProcent;
            Logger.ChangeTabloText -= Indicator.OnChangeTabloText;
            Logger.ChangePeriod -= Indicator.OnChangePeriod;
            Logger.SetProcentTimed -= Indicator.OnSetProcessTimed;
            Logger.SetProcentUsual -= Indicator.OnSetProcessUsual;
            Logger.ExecutionFinished -= OnExecutionFinished;
        }

        //Событие, сообщающее внешнему приложению, что выполнение было прервано
        public delegate void EvDelegate();
        public event EvDelegate Finished;
        
        //Обработка события прерывания
        private void OnExecutionFinished(object sender, EventArgs e)
        {
            if (Finished != null) Finished();
        }
    }
}