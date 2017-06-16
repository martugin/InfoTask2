using System;
using BaseLibrary;
using Calculation;
using Generator;

namespace Tablik
{
    //Проект для Таблика
    public class TablikProject : ExternalLogger
    {
        public TablikProject(DataProject project)
            : base(project.App, project.Code, project.Code)
        {
            Project = project;
            LoadAllConnects();
        }

        //Проект
        public DataProject Project { get; private set; }
        
        //Словарь модулей
        private readonly DicS<TablikModule> _modules = new DicS<TablikModule>();
        public IDicSForRead<TablikModule> Modules { get { return _modules; } }
        //Словарь источников
        private readonly DicS<TablikSource> _sources = new DicS<TablikSource>();
        public DicS<TablikSource> Sources { get { return _sources; } }
        //Словарь приемников
        private readonly DicS<TablikReceiver> _receivers = new DicS<TablikReceiver>();
        public DicS<TablikReceiver> Receivers { get { return _receivers; } }

        //Добавить модуль
        public TablikModule AddModule(string code)
        {
            return _modules.Add(code, new TablikModule(this, code));
        }
        //Очистить список молулей
        public void ClearModules()
        {
            _modules.Clear();
        }

        //Загрузить все провайдеры
        public void LoadAllConnects()
        {
            throw new NotImplementedException();
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