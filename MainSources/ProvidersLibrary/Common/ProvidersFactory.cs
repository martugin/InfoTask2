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

        //Список всех провайдеров из Config
        private readonly DicS<ProviderConfig> _providerConfigs = new DicS<ProviderConfig>();
        public DicS<ProviderConfig> ProviderConfigs { get { return _providerConfigs; } }

        //Создание провайдера
        public Provider CreateProv(Logger logger, //Логгер, например поток расчета
                                                 string code, //Код
                                                 string inf, string reserveInf = "") //Настройки основного и резервного подключения
        {
            return ProviderConfigs[code].RunProvider(logger, inf, reserveInf);
        }
    }
}