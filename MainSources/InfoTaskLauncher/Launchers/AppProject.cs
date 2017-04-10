using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;
using Generator;

namespace ComLaunchers
{
    //Проект, открытый в приложении
    internal class AppProject : Project
    {
        public AppProject(ItLauncher launcher, //Лаунчер
                                    string projectDir) //Каталог проекта
            : base(projectDir)
        {
            Launcher = launcher;
            Logger = ItStatic.CreateAppLogger(ProjectCode + "History.accdb");
        }

        //Тестовый вызов проекта, без указания проекта
        internal AppProject(ItLauncher launcher)
        {
            Launcher = launcher;
        }

        //Ссылка на Launcher
        public ItLauncher Launcher { get; private set; }
        
        //Генератор параметров
        internal TablGenerator Generator { get; private set; }

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (Generator == null)
                Generator = new TablGenerator(Logger);
            Logger.RunSyncCommand(() => Generator.GenerateParams(moduleDir));
        }
    }
}