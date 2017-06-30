using System.Collections.Generic;
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
        //Таблик
        public TablikApp App { get {return } }
        
        //Словарь модулей
        private readonly DicS<TablikModule> _modules = new DicS<TablikModule>();
        internal IDicSForRead<TablikModule> Modules { get { return _modules; } }
        //Список модулей в порядке обсчета
        private readonly List<TablikModule> _modulesOrder = new List<TablikModule>();
        internal List<TablikModule> ModulesOrder { get { return _modulesOrder; } }

        //Словарь источников
        private readonly DicS<TablikSource> _sources = new DicS<TablikSource>();
        internal DicS<TablikSource> Sources { get { return _sources; } }
        //Словарь приемников
        private readonly DicS<TablikReceiver> _receivers = new DicS<TablikReceiver>();
        internal DicS<TablikReceiver> Receivers { get { return _receivers; } }

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

        //Компиляция выбранных модулей
        public void Compile()
        {
            if (Modules.Count == 0) return;
            ModulesOrder.Clear();
            foreach (var m in Modules.Values)
                m.DfsStatus = DfsStatus.Before;
            foreach (var m in Modules.Values)
                if (m.DfsStatus == DfsStatus.Before)
                    MakeModuleGraph(m);

            foreach (var m in ModulesOrder)
                m.Compile();
        }

        //Определение порядка вычисления модулей
        internal void MakeModuleGraph(TablikModule m)
        {
            m.DfsStatus = DfsStatus.Process;
            foreach (var lm in m.LinkedModules)
            {
                if (lm.DfsStatus == DfsStatus.Before )
                    MakeModuleGraph(lm);
                else if (lm.DfsStatus == DfsStatus.Process)
                    AddWarning("Циклическая зависимость модулей", null, lm.Code);
            }
            ModulesOrder.Add(m);
            m.DfsStatus = DfsStatus.After;
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