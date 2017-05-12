using System;
using System.Collections.Generic;
using System.IO;
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
            if (projectDir.IsEmpty()) return;
            Dir = projectDir.EndDir();
            ReadProjectData();
            ReadLocalData();
        }

        //Загрузка данных из проекта
        private void ReadProjectData()
        {
            var elemRoot = XDocument.Load(Dir + "ProjectProperties.xml").Element("ProjectProperties");
            Initialize(elemRoot.GetAttr("ProjectCode"), elemRoot.GetAttr("ProjectName"));
            foreach (var elem in elemRoot.Element("Connects").Elements())
            {
                var con = new SchemeConnect(elem.GetName().ToProviderType(), elem.GetAttr("Code"), elem.GetAttr("Complect"), elem.GetAttr("Description"));
                Connects.Add(con.Code, con);
                if (con.Type == ProviderType.Source)
                    Sources.Add(con.Code, con);
                if (con.Type == ProviderType.Receiver)
                    Receivers.Add(con.Code, con);
            }
            var modulesDir = new DirectoryInfo(Dir + @"Modules\");
            foreach (var mdir in modulesDir.GetDirectories())
                Modules.Add(mdir.Name, new SchemeModule(this, mdir.Name));
        }

        //Загрузка настроек из LocalData
        private void ReadLocalData()
        {
            var elemRoot = XDocument.Load(LocalDir + "ConnectProperties.xml").Element("ConnectProperties");
            foreach (var elem in elemRoot.Elements())
            {
                var code = elem.GetName();
                if (Connects.ContainsKey(code))
                {
                    var con = Connects[code];
                    var el = con.Type == ProviderType.Source ? elem.Element("SourceProvider")
                             : (con.Type == ProviderType.Receiver ? elem.Element("ReceiverProvider") 
                             : null);
                    var prInf = "";
                    foreach (var prop in el.Elements())
                        prInf += prop.GetName() + "=" + prop.GetAttr("ProvValue") + ";";
                    con.JoinProviders(el.GetAttr("Provider"), prInf);
                }
            }
        }

        //Каталог проекта
        public string Dir { get; private set; }

        //Модули
        private readonly DicS<SchemeModule> _modules = new DicS<SchemeModule>();
        public DicS<SchemeModule> Modules { get { return _modules; } }

        //Полный список соединений
        private readonly DicS<SchemeConnect> _connects = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> Connects { get { return _connects; } }
        //Источники
        private readonly DicS<SchemeConnect> _sources = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> Sources { get { return _sources; } }
        //Приемники
        private readonly DicS<SchemeConnect> _receivers = new DicS<SchemeConnect>();
        public DicS<SchemeConnect> Receivers { get { return _receivers; } }
    }
}