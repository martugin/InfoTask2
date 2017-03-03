using ProvidersLibrary;

namespace ComClients
{
    //Интерфейс для ProvConnect
    public interface IProvConnect : ILoggerClient
    {
        //Код соединения
        string Name { get; }
        //Комплект провайдеров
        string Complect { get; }
        //Присвоение основного и резервного провайдера 
        void JoinProviders(string mainCode, string mainInf, string reserveCode = null, string reserveInf = null);
    }

    //-------------------------------------------------------------------------------------------------------------
    //Базовый класс для соединений с провайдерами через COM
    public abstract class ProvConnect : LoggerClient, IProvConnect
    {
        protected ProvConnect(ProviderConnect providerConnect, ProvidersFactory factory)
        {
            ProviderConnect = providerConnect;
            Factory = factory;
        }

        //Ссылка на соединение
        protected ProviderConnect ProviderConnect { get; private set; }
        //Фабрика провайдеров
        protected ProvidersFactory Factory { get; private set; }

        //Тип провайдера
        protected abstract ProviderType Type { get; }
        //Код соединения
        public string Name { get { return ProviderConnect.Name; } }
        //Комплект провайдеров
        public string Complect { get { return ProviderConnect.Complect; } }

        //Присвоение основного и резервного провайдера 
        public void JoinProviders(string mainCode, string mainInf, //Код и настройки основного провайдера
                                               string reserveCode = null, string reserveInf = null) //Код и настройки резервного провайдера
        {
            RunShortCommand(() =>
                {
                    var main = Factory.CreateProvider(mainCode, mainInf);
                    var reserve = reserveCode == null ? null : Factory.CreateProvider(reserveCode, reserveInf);
                    ProviderConnect.JoinProviders(main, reserve);
                });
        }
    }
}