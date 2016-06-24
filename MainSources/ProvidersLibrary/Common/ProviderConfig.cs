using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Описание одного провайдера из Config
    public class ProviderConfig
    {
        public ProviderConfig(ProviderType type, string code, ProvidersFactory factory)
        {
            Type = type;
            Code = code;
            Factory = factory;
        }

        //Ссылка на фабрику провайдеров
        public ProvidersFactory Factory { get; private set; }

        //Тип провайдера
        public ProviderType Type { get; private set; }
        //Код провайдера
        public string Code { get; private set; }
        //Пути к файлу и папке dll провайдера
        private string _file;
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                Dir = new FileInfo(_file).DirectoryName;
            }
        }
        public string Dir { get; private set; }

        //Запуск экземпляра провайдера через MEF, позднее связывание с dll
        [ImportMany(typeof(Prov))]
        private Lazy<Prov, IDictionary<string, object>>[] ImportProvs { get; set; }

        public Prov RunProv(string inf, Logger logger)
        {
            var prc = Factory.ProviderConfigs[Code];
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(prc.Dir));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            try
            {
                var pr = (from codeVault in ImportProvs
                          where (string)codeVault.Metadata["Code"] == Code
                          select codeVault).Single().Value;
                pr.Logger = logger;
                pr.Inf = inf;
                return pr;
            }
            catch { return null; }
        }
    }
}