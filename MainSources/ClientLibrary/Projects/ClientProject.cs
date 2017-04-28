using CommonTypes;

namespace ClientLibrary
{
    //Проект для клиента
    public class ClientProject : BaseProject
    {
        public ClientProject(BaseApp app, string projectCode)
            : base(app)
        {
            Initialize(projectCode);
        }
    }
}