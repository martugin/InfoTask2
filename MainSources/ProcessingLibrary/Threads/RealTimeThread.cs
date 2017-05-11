namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : BaseThread
    {
        public RealTimeThread(ProcessProject project, int id, string name)
            : base(project, id, name) { }

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