using CommonTypes;
using Generator;
using Tablik;

namespace Integrated
{
    //Проект, вызываемый из приложения
    public class AppProject : Project
    {
        public AppProject(App app)
            : base(app, app.Indicator)
        {
            
        }

        //Проект Таблика
        private TablikProject _tablik;
        public TablikProject Tablik 
        {
            get { return _tablik = _tablik ?? new TablikProject(this); }
        }

        //Ссылка на генератор параметров
        internal TablGenerator Generator { get; private set; }

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (Generator == null)
                Generator = new TablGenerator(this);
            RunSyncCommand(() => Generator.GenerateParams(moduleDir));
        }
    }
}