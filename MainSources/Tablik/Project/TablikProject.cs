using System;
using BaseLibrary;
using Calculation;
using Generator;

namespace Tablik
{
    //Проект для Таблика
    public class TablikProject : ExternalLogger
    {
        public TablikProject(SchemeProject project)
            : base(project.App, project.Code, project.Code)
        {
            Project = project;
            LoadAllSignals();
        }

        //Проект
        public SchemeProject Project { get; private set; }
        
        //Словарь модулей
        private readonly DicS<TablikModule> _modules = new DicS<TablikModule>();
        public IDicSForRead<TablikModule> Modules { get { return _modules; } }
        //Словарь источников
        private readonly DicS<TablikSource> _sources = new DicS<TablikSource>();
        public DicS<TablikSource> Sources { get { return _sources; } }
        //Словарь приемников
        private readonly DicS<TablikReceiver> _receivers = new DicS<TablikReceiver>();
        public DicS<TablikReceiver> Receivers { get { return _receivers; } }

        //Загрузить сигналы всех провайдеров
        public void LoadAllSignals()
        {
            foreach (var source in Sources.Values)
                source.LoadSignals();
            foreach (var receiver in Receivers.Values)
                receiver.LoadSignals();
        }

        //Загрузить сигналы указанного провайдера
        public void LoadSignals(string providerCode)
        {
            if (Sources.ContainsKey(providerCode))
                Sources[providerCode].LoadSignals();
            if (Receivers.ContainsKey(providerCode))
                Receivers[providerCode].LoadSignals();
        }

        //Очистить список модулей
        public void ClearModules()
        {
            _modules.Clear();
        }

        //Добавить модуль
        public void AddModule(string code)
        {
            if (!_modules.ContainsKey(code))
            {
                var mod = new TablikModule(this, code);
                _modules.Add(code, mod);
                var smod = Project.SchemeModules[code];
                foreach (var m in smod.LinkedModules.Values)
                    AddModule(m);    
            }
        }

        #region Generator
        //Ссылка на генератор параметров
        internal TablGenerator Generator { get; private set; }

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (Generator == null)
                Generator = new TablGenerator(Project.App);
            RunSyncCommand(() => Generator.GenerateParams(moduleDir));
        }
        #endregion
    }
}