using ProcessingLibrary;
using Tablik;

namespace AppLibrary
{
    //Проект, вызываемый из приложения
    public class AppProject : ProcessProject
    {
        public AppProject(App app, string projectDir)
            : base(app, projectDir)
        {
            Thread = new BaseThread(this, 1, "Поток");
            Threads.Add(1, Thread);
        }

        //Проект Таблика
        private TablikProject _tablik;
        public TablikProject Tablik 
        {
            get { return _tablik = _tablik ?? new TablikProject(this); }
        }

        //Поток расчетов
        public BaseThread Thread { get; private set; }
    }
}