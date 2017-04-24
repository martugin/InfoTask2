using CommonTypes;

namespace ClientLibrary
{
    //Проект для клиента
    public class ClientProject : Project
    {
        public ClientProject(BaseApp app, string projectCode)
            : base(app)
        {
            InitializeByCode(projectCode);
        }
    }
}