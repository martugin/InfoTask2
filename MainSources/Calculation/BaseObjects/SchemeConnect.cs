using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    public class SchemeConnect : ISchemeConnect
    {
        public SchemeConnect(ProviderType type, string code, string complect, string description = "")
        {
            Type = type;
            Code = code;
            Complect = complect;
            Description = description;
        }

        //Загрузить настройки провайдеров
        public void JoinProviders(string providerCode, string providerInf, string reserveCode = null, string reserveInf = null)
        {
            ProviderCode = providerCode;
            ProviderInf = providerInf;
            ReserveProviderCode = reserveCode;
            ReserveProviderInf = reserveInf;
        }

        //Код соединения
        public string Code { get; protected set; }
        //Тип провайдера
        public ProviderType Type { get; private set; }
        //Комплект провайдеров
        public string Complect { get; protected set; }
        //Описание
        public string Description { get; private set; }

        //Код и настройки основного провайдера
        public string ProviderCode { get; private set; }
        public string ProviderInf { get; private set; }
        //Код и настройки резервного провайдера
        public string ReserveProviderCode { get; private set; }
        public string ReserveProviderInf { get; private set; }

        //Связанные входящие и исходящие соединения
        //Связанные входящие и исходящие соединения
        private readonly List<ISchemeConnect> _inConnects = new List<ISchemeConnect>();
        public List<ISchemeConnect> InConnects { get { return _inConnects; } }
        private readonly List<ISchemeConnect> _outConnect = new List<ISchemeConnect>();
        public List<ISchemeConnect> OutConnect { get { return _outConnect; } }
    }
}