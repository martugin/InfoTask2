using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для соединений Таблика и расчетов
    public abstract class BaseConnect : ExternalLogger
    {
        protected BaseConnect(Project project, string name, string complect) 
            : base(project.App, name, project.Code)
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
    }
}