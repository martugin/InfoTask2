using System.Xml.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс проекта 
    public class Project : ExternalLogger
    {
        public Project(BaseApp app) : base(app)
        {
            App = app;
        }

        //Загрузить свойства из каталога проекта
        public void InitializeByDir(string projectDir)
        {
            Dir = projectDir;
            var elem = XDocument.Load(projectDir.EndDir() + "ProjectProperties.xml").Element("ProjectProperties");
            InitializeByCode(elem.GetAttr("ProjectCode"), elem.GetAttr("ProjectName"));
        }

        //Присвоить код и имя проекта
        public void InitializeByCode(string projectCode, string projectName = "")
        {
            Code = projectCode;
            Name = projectName;
            ProgressContext = Context = projectCode;
        }

        //Создание истории
        public IHistory CreateHistory(string fileName) //Добавка к имени файла
        {
            return new AccessHistory(AppCode + "\\" + Code + fileName, ItStatic.TemplatesDir + @"LocalData\History.accdb");
        }

        //Приложение
        public BaseApp App { get; private set; }
        //Код приложения
        public string AppCode { get { return App.Code; } }

        //Код и имя проекта
        public string Code { get; private set; }
        public string Name { get; private set; }
        //Каталог проекта
        public string Dir { get; private set; }
        //Каталог проекта в LocalData
        public string LocalDir
        {
            get { return ItStatic.InfoTaskDir() + @"LocalData\" + AppCode + @"\" + Code + @"\"; }
        }
    }
}