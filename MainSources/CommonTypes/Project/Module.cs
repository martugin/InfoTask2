using System.Xml.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Один модуль из проекта
    public abstract class Module : ExternalLogger
    {
        protected Module(ExternalProject project, string code) : base(project.Logger)
        {
            Code = code;
            Dir = project.Project.Dir.EndDir() + @"\Modules\" + code + "\\";
        }

        //Код модуля
        public string Code { get; private set; }
        //Имя и описание модуля
        public string Name { get; private set; }
        public string Description { get; private set; }
        //Каталог модуля в проекте
        public string Dir { get; private set; }

        //Загрузка свойств модуля
        public void LoadProperties()
        {
            var elem = XDocument.Load(Dir + "ModuleProperties.xml").Element("ModuleProperties");
            Name = elem.GetAttr("ModuleName");
            Description = elem.GetAttr("Deescription");
            var linked = elem.Element("LinkedModules").Elements();
            foreach (var el in linked)
                AddLinkedModule(el.Name.LocalName);
            linked = elem.Element("LinkedConnects").Elements();
            foreach (var el in linked)
                AddLinkedConnect(el.Name.LocalName);
        }

        //Добавить связанный модуль
        protected abstract void AddLinkedModule(string moduleCode);
        //Добавить связанное соединение
        protected abstract void AddLinkedConnect(string connectCode);
    }
}