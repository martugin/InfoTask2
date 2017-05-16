using BaseLibrary;
using Calculation;

namespace ProcessingLibrary
{
    //Проект с возможностью управления обработкой мгновенных данных (расчеты и т.п.)
    public class ProcessProject : SchemeProject
    {
        protected ProcessProject(ProcessApp app, string projectDir) 
            : base(app, projectDir) { }

        //Потоки расчета
        private readonly DicI<BaseThread> _threads = new DicI<BaseThread>();
        public DicI<BaseThread> Threads { get { return _threads; } }
    }
}