using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : BaseThread
    {
        public RealTimeThread(ProcessProject project, int id, string name, IIndicator indicator, LoggerStability stability = LoggerStability.RealTimeFast)
            : base(project, id, name)
        {
            ThreadLogger = new Logger(Project.App.CreateHistory(Project.Code + Id), indicator, stability); 
        }

        //Длительность одного цикла в секундах
        public double PeriodSeconds { get; set; }
        //Возможная задержка архивных источников в сукундах
        public double LateSeconds { get; set; }

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