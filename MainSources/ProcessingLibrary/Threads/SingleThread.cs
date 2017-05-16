using BaseLibrary;
using System;

namespace ProcessingLibrary
{
    //Поток разовых расчетов
    public class SingleThread : BaseThread
    {
        public SingleThread(ProcessProject project, int id, string name, IIndicator indicator) 
            : base(project, id, name, indicator, LoggerStability.Single) { }

        //Запуск обработки
        protected override void Run()
        {
            RunPrepare();
            RunCycle();
        }
    }
}