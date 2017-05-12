using System.Collections.Generic;
using System.Xml.Linq;
using BaseLibrary;

namespace Calculation
{
    public class SchemeModule : DataModule, ISchemeConnect
    {
        public SchemeModule(SchemeProject project, string code) 
            : base(project, code)
        {
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
        
        //Коды связанных модулей и соединений
        private readonly SetS _linkedModulesCodes = new SetS();
        public SetS LinkedModulesCodes { get { return _linkedModulesCodes; }}
        private readonly SetS _linkedConnectsCodes = new SetS();
        public SetS LinkedConnectsCodes { get { return _linkedConnectsCodes; }}
       
        //Связанные входящие и исходящие соединения
        private readonly List<ISchemeConnect> _inConnects = new List<ISchemeConnect>();
        public List<ISchemeConnect> InConnects { get { return _inConnects; } }
        private readonly List<ISchemeConnect> _outConnect = new List<ISchemeConnect>();
        public List<ISchemeConnect> OutConnect { get { return _outConnect; } }
    }
}