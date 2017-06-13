using System;
using System.Threading;
using BaseLibrary;
using CommonTypes;

namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : RealTimeBaseThread
    {
        public RealTimeThread(ProcessProject project, int id, string name, IIndicator indicator, double periodSeconds, double lateSeconds)
            : base(project, id, name, indicator)
        {
            PeriodSeconds = periodSeconds;
            LateSeconds = lateSeconds;
        }

        //Длительность одного цикла в секундах
        public double PeriodSeconds { get; set; }
        //Возможная задержка архивных источников в сукундах
        public double LateSeconds { get; set; }

        //Запуск процесса
        public void StartProcess()
        {
            StartProcess(DateTime.Now.AddSeconds(-LateSeconds-PeriodSeconds));
        }

        //Ожидание следующей обработки
        protected override void Waiting()
        {
            var timeout = (int)NextPeriodStart.Subtract(DateTime.Now).TotalMilliseconds;
            if (timeout > 0) Thread.Sleep(timeout);
        }

        //Определение первого периода 
        protected override bool FirstPeriod()
        {
            ThreadPeriodEnd = ThreadPeriodBegin.AddSeconds(PeriodSeconds);
            NextPeriodStart = ThreadPeriodEnd.AddSeconds(LateSeconds);
            return NextPeriodStart.Subtract(ThreadFinishTime).TotalSeconds < 0.0001;
        }

        //Определение следующего периода обработки, возвращает false, если следующй обработки не будет
        protected override bool NextPeriod()
        {
            ThreadPeriodBegin = ThreadPeriodBegin.AddSeconds(PeriodSeconds);
            return FirstPeriod();
        }
        
        //Цикл обработки
        protected override void Cycle()
        {
            using (StartPeriod(ThreadPeriodBegin, ThreadPeriodEnd, "RealTime"))
                RunCycle();
        }
    }
}