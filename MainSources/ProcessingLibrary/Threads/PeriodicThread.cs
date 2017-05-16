using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток расчетов
    public class PeriodicThread : BaseThread
    {
        public PeriodicThread(ProcessProject project, int id, string name)
            : base(project, id, name)
        {
        }

        //Длительность одного цикла в минутах
        public double PeriodMinutes { get; set; }
        //Возможная задержка источников в минутах
        public double LateMinutes { get; set; }

        protected override void RunCycle()
        {
            throw new System.NotImplementedException();
        }

        protected override void RunWaiting()
        {
            throw new System.NotImplementedException();
        }
    }
}