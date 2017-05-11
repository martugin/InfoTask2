namespace ProcessingLibrary
{
    //Поток расчетов
    public class CalcThread : BaseThread
    {
        public CalcThread(ProcessProject project, int id, string name) 
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