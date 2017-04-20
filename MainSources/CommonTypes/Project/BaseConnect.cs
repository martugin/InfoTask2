using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для соединений Таблика и расчетов
    public abstract class BaseConnect : ExternalLogger
    {
        protected BaseConnect(string name, string complect, Logger logger) : base(logger)
        {
            Name = name;
            Complect = complect;
        }

        //Код соединения
        public string Name { get; private set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public string Complect { get; private set; }
        //Контекст для логгера
        public override string Context
        {
            get { return Type.ToRussian() + ": " + Name; }
        }
    }
}