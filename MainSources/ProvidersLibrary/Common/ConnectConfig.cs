using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Описание одного соединения (комплекта провадеров, но одного типа) из Config
    public class ConnectConfig
    {
        public ConnectConfig(ProviderType type, string complect, ProvidersFactory factory)
        {
            Type = type;
            Complect = complect;
            Factory = factory;
        }

        //Ссылка на фабрику провайдеров
        public ProvidersFactory Factory { get; private set; }
        //Тип провайдера
        public ProviderType Type { get; private set; }
        //Комплект
        public string Complect { get; private set; }

        //Пути к папке комплекта провайдеров
        public string Dir { get; set; }

        //Запуск экземпляра провайдера через MEF, позднее связывание с dll
        [ImportMany(typeof(ProvConn))]
        private Lazy<ProvConn, IDictionary<string, object>>[] ImportConns { get; set; }

        public ProvConn RunConn(Logger logger)
        {
            var prc = Factory.ProviderConfigs[Complect];
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(prc.Dir));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            try
            {
                var conn = (from codeVault in ImportConns
                          where (string)codeVault.Metadata["Complect"] == Complect
                          select codeVault).Single().Value;
                conn.Logger = logger;
                return conn;
            }
            catch { return null; }
        }
    }
}