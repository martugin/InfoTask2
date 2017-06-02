using System;
using System.Threading;
using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток расчетов
    public class PeriodicThread : BaseThread
    {
        public PeriodicThread(ProcessProject project, int id, string name, IIndicator indicator, double periodMinutes, double lateMinutes)
            : base(project, id, name, indicator, LoggerStability.Periodic)
        {
            PeriodMinutes = periodMinutes;
            LateMinutes = lateMinutes;
        }

        //Длительность одного цикла в минутах
        public double PeriodMinutes { get; private set; }
        //Возможная задержка источников в минутах
        public double LateMinutes { get; private set; }

        //Текущий режим потока
        private string _threadPeriodMode = "";

        //Подготовка потока
        protected override void Prepare()
        {
            using (StartProgress("Подготовка потока"))
            {
                Start(0, 60).Run(LoadModules);
                Start(60, 80).Run(PrepareArchives);
                NextPeriodStart = ThreadPeriodEnd.AddMinutes(LateMinutes);
            }
        }

        //Подготовка архивов результатов
        protected void PrepareArchives()
        {
            throw new NotImplementedException();
        }

        //Ожидание следующей обработки
        protected override void Waiting()
        {
            StartLog("Опредение диапазона источников").Run(GetSourcesTime);
            if (NextPeriodStart <= DateTime.Now)
            {
                _threadPeriodMode = "Выравнивание";
                return;
            }
            var d = DateTime.Now;
            var dif = NextPeriodStart.Subtract(d).TotalSeconds;
            using (StartPeriod(ThreadPeriodBegin, ThreadPeriodEnd, _threadPeriodMode = "Синхронный"))
                using (StartProgress("Ожидание", "", "", NextPeriodStart))
                    while (d < NextPeriodStart)
                    {
                        if (CheckFinishing()) return;
                        Procent = 100.0 * DateTime.Now.Subtract(d).TotalSeconds / dif;
                        Thread.Sleep(1000);
                        d = DateTime.Now;
                    }
        }

        //Определение следующего периода обработки, возвращает false, если следующй обработки не будет
        protected override bool NextPeriod()
        {
            ThreadPeriodBegin = ThreadPeriodBegin.AddMinutes(PeriodMinutes);
            ThreadPeriodEnd = ThreadPeriodBegin.AddMinutes(PeriodMinutes);
            NextPeriodStart = ThreadPeriodEnd.AddMinutes(LateMinutes);
            return ThreadPeriodEnd.Subtract(ThreadFinishTime).TotalSeconds > 0.1;
        }
        
        //Цикл обработки
        protected override void Cycle()
        {
            using (StartPeriod(ThreadPeriodBegin, ThreadPeriodEnd, _threadPeriodMode))
                using (StartProgress("Расчет"))
                {
                    Start(0, 40).Run(ReadSources);
                    Start(40, 60).Run(ClaculateModules);
                    Start(60, 80).Run(WriteArchives);
                    Start(80, 90).Run(WriteReceivers);
                    Start(90, 100).Run(ClearValues);    
                }
        }

        //Запись в архивы результатов
        protected void WriteArchives()
        {
            throw new NotImplementedException();
        }
    }
}