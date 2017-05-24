using BaseLibrary;
using Calculation;

namespace ProcessingLibrary
{
    //Проект с возможностью управления обработкой мгновенных данных (расчеты и т.п.)
    public class ProcessProject : SchemeProject
    {
        protected ProcessProject(ProcessApp app, string projectDir, bool isTest) 
            : base(app, projectDir, isTest) { }

        //Потоки
        private readonly DicI<BaseThread> _threads = new DicI<BaseThread>();
        public DicI<BaseThread> Threads { get { return _threads; } }
    }
}