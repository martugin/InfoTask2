using System.Runtime.InteropServices;
using AppLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для LauncherProject
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherProject
    {
        //Код проекта
        string Code { get; }
        //Имя проекта
        string Name { get; }
        //Каталог проекта
        string Dir { get; }
        //Каталог локальных данных проекта
        string LocalDir { get; }

        //Генерация параметров
        void GenerateParams(string moduleDir);

        //Создание соединения
        LauncherSourceConnect CreateSourConnect(string name, //Имя соединения
                                                         string complect); //Комплект
        LauncherReceiverConnect CreateReceivConnect(string name, //Имя соединения
                                                              string complect); //Комплект
    }

    //---------------------------------------------------------------------------------------------------------
    //Проект InfoTask для использования в Access
    [ClassInterface(ClassInterfaceType.None)]
    public class LauncherProject : ILauncherProject
    {
        internal LauncherProject(AppProject project)
        {
            _project = project;
        }

        //Ссылка на проект
        private readonly AppProject _project;

        //Код проекта
        public string Code { get { return _project.Code; } }
        //Имя проекта
        public string Name { get { return _project.Name; } }
        //Каталог проекта
        public string Dir { get { return _project.Dir; } }
        //Каталог локальных данных проекта
        public string LocalDir { get { return _project.LocalDir; } }

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            _project.Tablik.GenerateParams(moduleDir);
        }

        //Создание соединения-источника
        public LauncherSourceConnect CreateSourConnect(string name, string complect)
        {
            SourceConnect s = null;
            _project.RunSyncCommand(() =>
            {
                s = new SourceConnect(_project.App, name, complect, _project.Code);
            });
            return new RLauncherSourceConnect(s, Factory);
        }

        //Создание соединения-приемника
        public LauncherReceiverConnect CreateReceivConnect(string name, string complect)
        {
            ReceiverConnect r = null;
            _project.RunSyncCommand(() =>
            {
                r = new ReceiverConnect(_project.App, name, complect, _project.Code);
            });
            return new RLauncherReceiverConnect(r, Factory);
        }

        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
        }
    }
}