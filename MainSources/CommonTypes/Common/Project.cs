using System;
using System.Xml.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс проекта 
    public class Project : IDisposable
    {
        public Project() { }

        public Project(string projectDir) //Каталог проекта
        {
            ProjectDir = projectDir;
            var elem = XDocument.Load(projectDir.EndDir() + "Config.xml").Element("ProjectProperties");
            ProjectCode = elem.GetAttr("ProjectCode");
            ProjectName = elem.GetAttr("ProjectName");
        }

        //Код и имя проекта
        public string ProjectCode { get; protected set; }
        public string ProjectName { get; protected set; }
        //Каталог проекта
        public string ProjectDir { get; private set; }

        //Ссылка на логгер
        public Logger Logger { get; protected internal set; }

        public void Dispose()
        {
            if (Logger != null)
                Logger.Dispose();
        }
    }
}