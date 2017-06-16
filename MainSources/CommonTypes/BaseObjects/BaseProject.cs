using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс проекта 
    public abstract class BaseProject : ExternalLogger
    {
        protected BaseProject(BaseApp app) : base(app)
        {
            App = app;
        }

        //Присвоить код и имя проекта
        protected internal void Initialize(string projectCode, string projectName)
        {
            Code = projectCode;
            Name = projectName;
            ProgressContext = Context = projectCode;
            LocalDir = App.LocalDir + Code + @"\";
            Static.CopyDir(ItStatic.TemplatesDir + @"LocalData\" + App.Code, LocalDir, false);
        }

        //Приложение
        public BaseApp App { get; private set; }

        //Код и имя проекта
        public string Code { get; private set; }
        public string Name { get; private set; }

        //Каталог проекта в LocalData
        public string LocalDir { get; protected set; }
    }
}