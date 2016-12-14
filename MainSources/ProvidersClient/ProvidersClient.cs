using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComClients
{
    //Клиент работы с провайдерами, вызываемый из внешних приложений через COM
    public class ProvidersClient : LoggerClient
    {
        //Инициализация
        public void Initialize(string application, //Код приложения
                                        string project) //Код проекта
        {
            Logger.OpenHistory(DifferentIt.LocalDataProjectDir(project) + @"History\" + application + @"\History.accdb", DifferentIt.TemplatesDir() + @"LocalData\History\History.accdb");
        }

        //Закрытие клиента
        public void Close()
        {
            GC.Collect();
            IsClosed = true;
        }

        //Клиент уже был закрыт
        protected bool IsClosed { get; private set; }
        
        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory 
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
        }

        //Создание соединения-источника
        public SourConnect CreateSourConnect(string name, string complect)
        {
            return new SourConnect(
                (SourceConnect)Factory.CreateConnect(ProviderType.Source, name, complect, Logger), 
                Factory);
        }

        //Создание соединения-приемника
        public ReceivConnect CreateReceivConnect(string name, string complect)
        {
            return new ReceivConnect(
                (ReceiverConnect)Factory.CreateConnect(ProviderType.Receiver, name, complect, Logger),
                Factory);
        }
    }
}