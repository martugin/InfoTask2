using System.Runtime.InteropServices;
using AppLibrary;

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
            //_project.Tablik.GenerateParams(moduleDir);
        }
    }
}