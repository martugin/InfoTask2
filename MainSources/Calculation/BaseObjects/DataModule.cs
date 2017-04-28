using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public abstract class DataModule : BaseModule
    {
        protected DataModule(DataProject project, string code) 
            : base(project, code)
        {
            Dir = project.Dir.EndDir() + @"\Modules\" + code + "\\";
            var elem = XDocument.Load(Dir + "ModuleProperties.xml").Element("ModuleProperties");
            Name = elem.GetAttr("ModuleName");
            Description = elem.GetAttr("Description");
            var linked = elem.Element("LinkedModules").Elements();
            foreach (var el in linked)
                LinkedModulesCodes.Add(el.Name.LocalName);
            linked = elem.Element("LinkedConnects").Elements();
            foreach (var el in linked)
                LinkedConnectsCodes.Add(el.Name.LocalName);
        }

        //Каталог модуля в проекте
        public string Dir { get; private set; }

        //Коды связанных модулей и соединений
        private readonly SetS _linkedModulesCodes = new SetS();
        public SetS LinkedModulesCodes { get { return _linkedModulesCodes; }}
        private readonly SetS _linkedConnectsCodes = new SetS();
        public SetS LinkedConnectsCodes { get { return _linkedConnectsCodes; }}
    }
}