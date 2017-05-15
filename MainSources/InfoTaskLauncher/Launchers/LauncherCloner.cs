using System;
using System.Runtime.InteropServices;
using AppLibrary;
using BaseLibrary;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для RLauncherCloner
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherCloner
    {
        //Код соединения
        string Name { get; }
        //Комплект провайдеров
        string Complect { get; }
        //Присвоение основного и резервного провайдера 
        void JoinProvider(string mainCode, string mainInf, string reserveCode = null, string reserveInf = null);

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
        
        //Код соединения
        public string Name { get { return Connect.Code; } }
        //Комплект провайдеров
        public string Complect { get { return Connect.Complect; } }

        //Присвоение основного и резервного провайдера 
        public void JoinProvider(string mainCode, string mainInf, //Код и настройки основного провайдера
                                             string reserveCode = null, string reserveInf = null) //Код и настройки резервного провайдера
        {
            App.RunSyncCommand(() =>
            {
                var main = App.ProvidersFactory.CreateProvider(App, mainCode, mainInf);
                var reserve = reserveCode == null ? null : App.ProvidersFactory.CreateProvider(App, reserveCode, reserveInf);
                Connect.JoinProvider(main, reserve);
            });
        }

        //Получение диапазона времени источника
        public void GetTime()
        {
            App.RunSyncCommand(() => { _interval = Connect.GetTime(); });
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