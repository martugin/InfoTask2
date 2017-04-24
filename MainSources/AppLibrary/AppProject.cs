using CommonTypes;
using Tablik;

namespace AppLibrary
{
    //Проект, вызываемый из приложения
    public class AppProject : ServerProject
    {
        public AppProject(App app, string projectDir)
            : base(app, projectDir) { }

        //Проект Таблика
        private TablikProject _tablik;
        public TablikProject Tablik 
        {
            get { return _tablik = _tablik ?? new TablikProject(this); }
        }

        //Расчетный проект
        private AppCalcProject _calc;
        public AppCalcProject Calc
        {
            get { return _calc = _calc ?? new AppCalcProject(this); }
        }

        //Проект реального времени
        private AppRealTimeProject _realTime;
        public AppRealTimeProject RealTime
        {
            get { return _realTime = _realTime ?? new AppRealTimeProject(this); }
        }
    }
}