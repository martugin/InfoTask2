namespace CommonTypes
{
    //Проект для приложений и сервисов
    public class ServerProject : Project
    {
        public ServerProject(BaseApp app, string projectDir)
            : base(app)
        {
            InitializeByDir(projectDir);
        }
    }
}