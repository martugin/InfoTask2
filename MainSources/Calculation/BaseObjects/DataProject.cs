using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Проект с известным каталогом
    public class DataProject : BaseProject
    {
        public DataProject(BaseApp app, string projectDir) : base(app)
        {
            Dir = projectDir.EndDir();
            var elem = XDocument.Load(projectDir + "ProjectProperties.xml").Element("ProjectProperties");
            Initialize(elem.GetAttr("ProjectCode"), elem.GetAttr("ProjectName"));
        }

        //Каталог проекта
        public string Dir { get; private set; }
    }
}