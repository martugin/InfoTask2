using System;
using System.Runtime.InteropServices;
using AppLibrary;
using BaseLibrary;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для LauncherCloner
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherCloner
    {
        //Получение диапазона времени источника
        void GetTime();

        //Диапазон времени источника
        DateTime BeginTime { get; }
        DateTime EndTime { get; }

        //Создание клона синхронно
        void MakeCloneSync(string cloneDir); //Каталог клона
        //Создание клона асинхронно
        void MakeCloneAsync(string cloneDir); //Каталог клона
    }

    //-------------------------------------------------------------------------------------
    //Обертка соединения для получения клона
    [ClassInterface(ClassInterfaceType.None)]
    public class LauncherCloner : ILauncherCloner
    {
        internal LauncherCloner(ClonerConnect connect)
        {
            Connect = connect;
        }

        //Ссылка на соединение
        internal ClonerConnect Connect { get; private set; }
        //Ссылка на приложение
        private App App { get { return (App)Connect.Logger; } }

        //Получение диапазона времени источника
        public void GetTime()
        {
            App.RunSyncCommand(() =>
            {
                using (Connect.StartLog("Определение диапазона источника"))
                    _interval = Connect.GetTime();
            });
        }
        private TimeInterval _interval = new TimeInterval();

        //Диапазон времени источника
        public DateTime BeginTime { get { return _interval.Begin; } }
        public DateTime EndTime { get { return _interval.End; } }

        //Создание клона синхронно
        public void MakeCloneSync(string cloneDir) //Каталог клона
        {
            Connect.MakeCloneSync(cloneDir);
        }

        //Создание клона асинхронно
        public void MakeCloneAsync(string cloneDir) //Каталог клона
        {
            Connect.MakeCloneAsync(cloneDir);
        }
    }
}