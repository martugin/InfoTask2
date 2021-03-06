﻿using BaseLibrary;
using Generator;
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
            get { return _tablik = _tablik ?? new TablikProject(this, ((App)App).FunsChecker); }
        }

        //Генератор
        internal TablGenerator Generator { get; private set; }
        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (Generator == null)
                Generator = new TablGenerator(App);
            RunSyncCommand(() => Generator.GenerateParams(moduleDir));
        }

        //Открыть разовый поток
        public SingleThread OpenSingleThread(int id, string name)
        {
            var t = new SingleThread(this, id, name, new AppIndicator());
            Threads.Add(id, t, true);
            return t;
        }

        //Открыть периодический поток
        public PeriodicThread OpenPeriodicThread(int id, string name, double periodMinutes, double lateMainutes = 0)
        {
            var t = new PeriodicThread(this, id, name, new AppIndicator(), periodMinutes, lateMainutes);
            Threads.Add(id, t, true);
            return t;
        }
    }
}