using System;
using System.Threading;
using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : BaseThread
    {
        public RealTimeThread(ProcessProject project, int id, string name, IIndicator indicator, double periodSeconds, double lateSeconds)
            : base(project, id, name, indicator, LoggerStability.RealTimeFast)
        {
            PeriodSeconds = periodSeconds;
            LateSeconds = lateSeconds;
        }

        //Длительность одного цикла в секундах
        public double PeriodSeconds { get; set; }
        //Возможная задержка архивных источников в сукундах
        public double LateSeconds { get; set; }
        
        //Подготовка потока
        #region Prepare
        protected override void Prepare()
        {
            using (StartProgress("Подготовка потока"))
            {
                Start(0, 60).Run(LoadModules);
            }
        }
        #endregion

        //Ожидание следующей обработки
        #region Waiting
        protected override void Waiting()
        {
            var timeout = (int)NextPeriodStart.Subtract(DateTime.Now).TotalMilliseconds;
            if (timeout > 0) Thread.Sleep(timeout);
        }

        //Определение следующего периода обработки, возвращает false, если следующй обработки не будет
        protected override bool NextPeriod()
        {
            ThreadPeriodBegin = ThreadPeriodBegin.AddSeconds(PeriodSeconds);
            ThreadPeriodEnd = ThreadPeriodBegin.AddSeconds(PeriodSeconds);
            NextPeriodStart = ThreadPeriodEnd.AddMinutes(LateSeconds);
            return NextPeriodStart.Subtract(ThreadFinishTime).TotalSeconds > 0.1;
        }
        #endregion

        //Цикл обработки
        #region Cycle
        protected override void Cycle()
        {
            using (StartPeriod(ThreadPeriodBegin, ThreadPeriodEnd, "RealTime"))
            {
                Start(0, 50).Run(ReadSources);
                Start(50, 60).Run(ClaculateModules);
                Start(60, 80).Run(WriteReceivers);
                Start(60, 80).Run(WriteProxies);
            }
        }
        #endregion
    }
}