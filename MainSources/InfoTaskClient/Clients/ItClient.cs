using System;
using System.Runtime.InteropServices;
using BaseLibrary;
using CommonTypes;
using Generator;
using ProvidersLibrary;

namespace ComClients
{
    //Интерфейс для ItClient
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IItClient : ILoggerClient
    {
        //Инициализация
        void Initialize(string appCode, //Код приложения
                              string project); //Код проекта
        //Закрытие клиента
        void Close();

        //Генерация параметров
        void GenerateParams(string moduleDir);

        //Создание соединения
        SourConnect CreateSourConnect(string name, //Имя соединения
                                                        string complect); //Комплект
        ReceivConnect CreateReceivConnect(string name, //Имя соединения
                                                              string complect); //Комплект
    }

    //------------------------------------------------------------------------------------------------------------------------------

    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    [ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(ILoggerClientEvents))]
    public class ItClient : LoggerClient , IItClient
    {
        //Инициализация
        public void Initialize(string appCode, //Код приложения
                                        string project) //Код проекта
        {
            AppCode = appCode;
            Project = project;
            Logger.History = new AccessHistory(Logger, DifferentIt.LocalDataProjectDir(project) + @"History\" + appCode + @"\History.accdb", DifferentIt.HistoryTemplateFile);
        }
        
        //Инициализация для запуска в тестах
        internal void InitializeTest()
        {
            AppCode = "Test";
            Project = "TestProject";
            Logger.History = new TestHistory(Logger);
        }

        //Код приложения
        protected string AppCode { get; private set; }
        //Код проекта
        protected string Project { get; private set; }

        //Генератор параметров, синглетон
        private TablGenerator _generator;

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (_generator == null)
                _generator = new TablGenerator(Logger);
            RunSyncCommand(() => _generator.GenerateParams(moduleDir));
        }

        //Создание соединения-источника
        public SourConnect CreateSourConnect(string name, string complect)
        {
            SourceConnect s = null;
            RunSyncCommand(() => { 
                s = (SourceConnect) Factory.CreateConnect(ProviderType.Source, name, complect, Logger);
            });
            return new SourConnect(s, Factory);
        }

        //Создание соединения-приемника
        public ReceivConnect CreateReceivConnect(string name, string complect)
        {
            ReceiverConnect r = null;
            RunSyncCommand(() => {
                r = (ReceiverConnect)Factory.CreateConnect(ProviderType.Receiver, name, complect, Logger);
            });
            return new ReceivConnect(r, Factory);
        }

        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
        }
    }
}
