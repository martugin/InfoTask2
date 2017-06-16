using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Проект, содержащий схему взаимодействия соединений и модулей
    public class SchemeProject : BaseProject
    {
        protected SchemeProject(BaseApp app, //Приложение
                                              string projectDir) //Каталог проекта
            : base(app)
        {
            if (projectDir.IsEmpty()) return;
            try
            {
                Dir = projectDir.EndDir();
                ReadProjectData();
                ReadLocalData();
            }
            catch (BreakException ex)
            {
                AddError("Ошибка при загрузке схемы проекта", ex);
            }
        }

        //Загрузка данных из проекта
        private void ReadProjectData()
        {
            AddEvent("Загрузка схемы проекта");
            var elemRoot = XDocument.Load(Dir + "ProjectProperties.xml").Element("ProjectProperties");
            Initialize(elemRoot.GetAttr("ProjectCode"), elemRoot.GetAttr("ProjectName"));
            foreach (var elem in elemRoot.Element("Connects").Elements())
            {
                var con = new SchemeConnect(elem.GetName().ToProviderType(), elem.GetAttr("Code"), elem.GetAttr("Complect"), elem.GetAttr("Description"));
                SchemeConnects.Add(con.Code, con);
                if (con.Type == ProviderType.Source) SchemeSources.Add(con.Code, con);
                if (con.Type == ProviderType.Receiver) SchemeReceivers.Add(con.Code, con);
                if (con.Type == ProviderType.Archive) SchemeArchives.Add(con.Code, con);
            }
            if (elemRoot.Elements("Proxies").Any())
                foreach (var elem in elemRoot.Element("Proxies").Elements())
                {
                    var proxy = new SchemeProxy(elem.GetName().ToProviderType(), elem.GetAttr("Code"));
                    foreach (var el in elem.Element("InConnects").Elements())
                        proxy.InConnects.Add(el.GetName());
                    foreach (var el in elem.Element("OutConnects").Elements())
                        proxy.OutConnects.Add(el.GetName());
                }

            var modulesDir = new DirectoryInfo(Dir + @"Modules\");
            if (modulesDir.Exists)
                foreach (var mdir in modulesDir.GetDirectories())
                    SchemeModules.Add(mdir.Name, new SchemeModule(this, mdir.Name));
        }

        //Загрузка настроек из LocalData
        private void ReadLocalData()
        {
            AddEvent("Загрузка настроек проекта");
            var elemRoot = XDocument.Load(LocalDir + "ConnectProperties.xml").Element("ConnectProperties");
            foreach (var elem in elemRoot.Elements())
            {
                var code = elem.GetName();
                if (SchemeConnects.ContainsKey(code))
                {
                    var con = SchemeConnects[code];
                    var el = con.Type == ProviderType.Source ? elem.Element("SourceProvider")
                             : (con.Type == ProviderType.Receiver ? elem.Element("ReceiverProvider") 
                             : null);
                    if (el != null)
                    {
                        var prInf = "";
                        foreach (var prop in el.Elements())
                            prInf += prop.GetName() + "=" + prop.GetAttr("PropValue") + ";";
                        con.JoinProviders(el.GetAttr("Provider"), prInf);    
                    }
                }
            }
        }

        //Каталог проекта
        public string Dir { get; private set; }

        //Модули
        private readonly DicS<SchemeModule> _schemeModules = new DicS<SchemeModule>();
        public DicS<SchemeModule> SchemeModules { get { return _schemeModules; } }

        //Полный список соединений
        private readonly DicS<SchemeConnect> _schemeConnects = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> SchemeConnects { get { return _schemeConnects; } }
        //Источники
        private readonly DicS<SchemeConnect> _schemeSources = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> SchemeSources { get { return _schemeSources; } }
        //Приемники
        private readonly DicS<SchemeConnect> _schemeReceivers = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> SchemeReceivers { get { return _schemeReceivers; } }
        //Архивы
        private readonly DicS<SchemeConnect> _schemeArchives = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> SchemeArchives { get { return _schemeArchives; } }
        //Прокси
        private readonly DicS<SchemeConnect> _schemeProxies = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> SchemeProxies { get { return _schemeProxies; } }
    }
}