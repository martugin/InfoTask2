using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Xml.Linq;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Создание провайдеров и соединений
    public class ProvidersFactory
    {
        public ProvidersFactory()
        {
            //Загрузить провайдеры из Config
            var doc = XDocument.Load(DifferentIT.InfoTaskDir() + @"General\Config.xml");
            foreach (var ncomp in doc.Element("Config").Element("Providers").Elements())
            {
                var ccode = ncomp.GetName();
                var complect = new ComplectConfig(ccode, ncomp.GetAttr("DllFile"));
                ComplectConfigs.Add(ccode, complect);
                foreach (var nprov in ncomp.Elements())
                {
                    var pcode = nprov.GetName();
                    var provider = new ProviderConfig(complect, nprov.GetAttr("ProviderType").ToProviderType(), pcode);
                    ProviderConfigs.Add(pcode, provider);
                }
            }
        }

        //Список всех провайдеров из Config
        private readonly DicS<ProviderConfig> _providerConfigs = new DicS<ProviderConfig>();
        public DicS<ProviderConfig> ProviderConfigs { get { return _providerConfigs; } }
        //Список всех комплектов из Config
        private readonly DicS<ComplectConfig> _complectConfigs = new DicS<ComplectConfig>();
        public DicS<ComplectConfig> ComplectConfigs { get { return _complectConfigs; } }

        //Создание провайдера
        public ProviderBase CreateProvider(Logger logger, //Логгер, например поток расчета
                                                 string code, //Код провайдера
                                                 string name, //Имя соединения
                                                 string inf, string reserveInf = null) //Настройки основного и резервного подключения
        {
            var prc = ProviderConfigs[code];
            var pr = prc.Complect.Complect == "Clones" || prc.Complect.Complect == "Archives"
                         ? NewStandardProvider(prc)
                         : NewProvider(prc);

            pr.Name = name;
            pr.Logger = logger;
            pr.AddMainConnect(inf);
            if (reserveInf != null) pr.AddReserveConnect(reserveInf);
            return pr;
        }

        //Запуск экземпляра провайдера через MEF, позднее связывание с dll
        [ImportMany(typeof(ProviderBase))]
        private Lazy<ProviderBase, IDictionary<string, object>>[] ImportProvs { get; set; }

        //Создать провайдер через MEF
        private ProviderBase NewProvider(ProviderConfig prc)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(prc.Complect.DllDir));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            try
            {
                return (from codeVault in ImportProvs
                          where (string)codeVault.Metadata["Code"] == prc.Code
                          select codeVault).Single().Value;
            }
            catch { return null; }
        }

        //Создать встроенный провайдер
        private ProviderBase NewStandardProvider(ProviderConfig prc)
        {
            switch (prc.Code)
            {
                case "CloneSource":
                    return new CloneSource();
            }
            return null;
        }
    }
}