namespace ProcessingLibrary
{
    //Поток расчетов
    public class CalcThread : BaseThread
    {
        public CalcThread(ProcessProject project, int id, string name) 
            : base(project, id, name) { }
    }
}