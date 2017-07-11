using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Проект для Таблика
    public class TablikProject : ExternalLogger
    {
        public TablikProject(SchemeProject project, FunsChecker funsChecker)
            : base(project.App, project.Code, project.Code)
        {
            Project = project;
            FunsChecker = funsChecker;
            LoadAllSignals();
        }

        //Проект
        public SchemeProject Project { get; private set; }
        //Проверка функций
        public FunsChecker FunsChecker { get; private set; }
        
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
            Sources.Clear();
            Receivers.Clear();
            foreach (var s in Project.SchemeSources.Values)
            {
                Sources.Add(s.Code, new TablikSource(this, s.Code, s.Complect));
                Sources[s.Code].LoadSignals();
            }
            foreach (var r in Project.SchemeReceivers.Values)
            {
                Receivers.Add(r.Code, new TablikReceiver(this, r.Code, r.Complect));
                Receivers[r.Code].LoadSignals();
            }
        }

        //Загрузить сигналы указанного провайдера
        public void LoadSignals(string connectCode)
        {
            if (Project.SchemeSources.ContainsKey(connectCode))
            {
                if (!Sources.ContainsKey(connectCode))
                    Sources.Add(connectCode, new TablikSource(this, connectCode, Project.SchemeSources[connectCode].Complect));
                Sources[connectCode].LoadSignals();
            }
            if (Project.SchemeReceivers.ContainsKey(connectCode))
            {
                if (!Receivers.ContainsKey(connectCode))
                    Receivers.Add(connectCode, new TablikReceiver(this, connectCode, Project.SchemeReceivers[connectCode].Complect));
                Receivers[connectCode].LoadSignals();
            }
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
                {
                    AddModule(m);
                    mod.LinkedModules.Add(_modules[m]);
                }
                foreach (var c in smod.LinkedConnects.Values)
                {
                    if (Project.SchemeSources.ContainsKey(c))
                        mod.LinkedSources.Add(Sources[c]);
                    if (Project.SchemeReceivers.ContainsKey(c))
                        mod.LinkedReceivers.Add(Receivers[c]);
                }
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
    }
}