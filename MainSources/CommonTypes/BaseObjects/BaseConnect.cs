using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для соединений Таблика и расчетов
    public abstract class BaseConnect : ExternalLogger
    {
        protected BaseConnect(BaseProject project, string code, string complect) 
            : base(project.App, code, project.Code)
        {
            Code = code;
            Complect = complect;
        }

        //Код соединения
        public string Code { get; private set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public string Complect { get; private set; }
    }
}