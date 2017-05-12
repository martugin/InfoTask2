using System.Collections.Generic;
using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Проект, содержащий схему взаимодействия соединений и модулей
    public class SchemeProject : BaseProject
    {
        protected SchemeProject(BaseApp app, string projectDir)
            : base(app)
        {
            Dir = projectDir;
            if (!Dir.IsEmpty())
            {
                var elem = XDocument.Load(projectDir.EndDir() + "ProjectProperties.xml").Element("ProjectProperties");
                Initialize(elem.GetAttr("ProjectCode"), elem.GetAttr("ProjectName"));    
            }
        }

        //Каталог проекта
        public string Dir { get; private set; }

        //Модули
        private readonly List<SchemeModule> _modules = new List<SchemeModule>();
        public List<SchemeModule> Modules { get { return _modules; } }

        //Источники
        private readonly List<SchemeConnect> _sources = new List<SchemeConnect>();
        public List<SchemeConnect> Sources { get { return _sources; } }
        //Приемники
        private readonly List<SchemeConnect> _receivers = new List<SchemeConnect>();
        public List<SchemeConnect> Receivers { get { return _receivers; } }
    }
}