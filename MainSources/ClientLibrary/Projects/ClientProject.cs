using CommonTypes;

namespace ClientLibrary
{
    //Проект для клиента
    public class ClientProject : BaseProject
    {
        public ClientProject(BaseApp app, string projectCode, string projectName)
            : base(app)
        {
            Initialize(projectCode, projectName);
        }
    }
}