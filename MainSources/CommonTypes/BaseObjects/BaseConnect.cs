using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для соединений Таблика и расчетов
    public abstract class BaseConnect : ExternalLogger
    {
        protected BaseConnect(Logger logger, string code, string complect, string projectCode) 
            : base(logger, code, projectCode)
        {
            Code = code;
            Complect = complect;
        }

        //Код соединения
        public string Code { get; protected set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public string Complect { get; protected set; }
    }
}