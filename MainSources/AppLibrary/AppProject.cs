using Calculation;
using ProcessingLibrary;
using Tablik;

namespace AppLibrary
{
    //Проект, вызываемый из приложения
    public class AppProject : ProcessProject
    {
        public AppProject(App app, string projectDir)
            : base(app, projectDir) { }

        //Проект Таблика
        private TablikProject _tablik;
        public TablikProject Tablik 
        {
            get { return _tablik = _tablik ?? new TablikProject(this); }
        }
    }
}