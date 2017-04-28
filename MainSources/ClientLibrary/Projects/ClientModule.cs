using CommonTypes;

namespace ClientLibrary
{
    //Представление модуля в клиенте
    public class ClientModule : BaseModule
    {
        public ClientModule(BaseProject project, string code) 
            : base(project, code) { }
    }
}