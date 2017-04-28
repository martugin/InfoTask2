namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : BaseThread
    {
        public RealTimeThread(ProcessProject project, int id, string name)
            : base(project, id, name) { }
    }
}