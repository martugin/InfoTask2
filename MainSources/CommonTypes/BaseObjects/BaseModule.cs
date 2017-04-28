using BaseLibrary;

namespace CommonTypes
{
    //Один модуль из проекта
    public abstract class BaseModule : ExternalLogger
    {
        protected BaseModule(BaseProject project, string code) 
            : base(project.App, code, project.Code)
        {
            Code = code;
        }

        //Код модуля
        public string Code { get; protected set; }
        //Имя и описание модуля
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        //Тип соединения
        public ProviderType Type { get { return ProviderType.Module; } }
    }
}