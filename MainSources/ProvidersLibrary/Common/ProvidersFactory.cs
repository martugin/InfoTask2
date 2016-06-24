using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Создание провайдеров и соединений
    public class ProvidersFactory
    {
        public ProvidersFactory()
        {
            //Загрузить провайдеры из Config
            throw new NotImplementedException();
        }

        //Список всех комплектов из Config
        private readonly DicS<ConnectConfig> _connectConfigs = new DicS<ConnectConfig>();
        public DicS<ConnectConfig> ConnectConfigs { get { return _connectConfigs; } }
        //Список всех провайдеров из Config
        private readonly DicS<ProviderConfig> _providerConfigs = new DicS<ProviderConfig>();
        public DicS<ProviderConfig> ProviderConfigs { get { return _providerConfigs; } }

        //Создание соединения
        public ProvConn CreateConn(string name, //Имя
                                                  string complect, //Комплект
                                                  Logger logger) //Логгер, например поток расчета
        {
            var conn = ConnectConfigs[complect].RunConn(logger);
            conn.Name = name;
            return conn;
        }

        //Создание провайдера
        public Prov CreateProv(string code, //Код
                                                  string inf, //Настройки 
                                                  Logger logger) //Логгер, например поток расчета
        {
            var prov = ProviderConfigs[code].RunProv(inf, logger);
            prov.Inf = inf;
            return prov;
        }
    }
}