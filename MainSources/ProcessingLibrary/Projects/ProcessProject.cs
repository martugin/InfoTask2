using BaseLibrary;
using CompileLibrary;

namespace ProcessingLibrary
{
    //Проект с возможностью управления обработкой мгновенных данных (расчеты и т.п.)
    public class ProcessProject : SchemeProject
    {
        protected ProcessProject(ProcessApp app, string projectDir) 
            : base(app, projectDir) { }

        //Потоки
        private readonly DicI<BaseThread> _threads = new DicI<BaseThread>();
        public DicI<BaseThread> Threads { get { return _threads; } }
    }
}