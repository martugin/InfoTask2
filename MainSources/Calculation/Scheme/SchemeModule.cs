using System.Xml.Linq;
using BaseLibrary;

namespace Calculation
{
    //Модуль для схемы проекта
    public class SchemeModule : DataModule
    {
        public SchemeModule(SchemeProject project, string code) 
            : base(project, code)
        {
            var elem = XDocument.Load(Dir + "ModuleProperties.xml").Element("ModuleProperties");
            Name = elem.GetAttr("ModuleName");
            Description = elem.GetAttr("Description");
            var linked = elem.Element("LinkedModules").Elements();
            foreach (var el in linked)
                LinkedModules.Add(el.Name.LocalName);
            linked = elem.Element("LinkedConnects").Elements();
            foreach (var el in linked)
                LinkedConnects.Add(el.Name.LocalName);
        }
        
        //Коды связанных модулей и соединений
        private readonly SetS _linkedModules = new SetS();
        public SetS LinkedModules { get { return _linkedModules; }}
        private readonly SetS _linkedConnects = new SetS();
        public SetS LinkedConnects { get { return _linkedConnects; }}
    }
}