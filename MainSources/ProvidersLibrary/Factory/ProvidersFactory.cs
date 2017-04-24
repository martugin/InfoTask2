﻿using System;
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
            var doc = XDocument.Load(ItStatic.InfoTaskDir() + @"General\Config.xml");
            foreach (var ncomp in doc.Element("Config").Element("Providers").Elements())
            {
                var ccode = ncomp.GetName();
                var complect = new ComplectConfig(ccode, ncomp.GetAttr("DllFile"));
                ComplectConfigs.Add(ccode, complect);
                foreach (var nprov in ncomp.Elements())
                {
                    var pcode = nprov.GetName();
                    var provider = new ProviderConfig(complect, 
                                                                       nprov.GetAttr("ProviderType").ToProviderType(), 
                                                                       nprov.GetAttr("SignalType").ToSignalType(), 
                                                                       pcode);
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

        //Создание соединения
        public ProviderConnect CreateConnect(Project project, //Проект
                                                                 ProviderType type, //Тип провайдера
                                                                 string name, //Имя соединения
                                                                 string complect) //Комплект
        {
            switch (type)
            {
                case ProviderType.Source:
                    return new SourceConnect(project, name, complect);
                case ProviderType.Receiver:
                    return new ReceiverConnect(project, name, complect);
            }
            return null;
        }

        //Создание провайдера
        public Provider CreateProvider(string code, //Код провайдера
                                                       string inf, //Настройки
                                                       Project project) //Проект
        {
            var prc = ProviderConfigs[code];
            var pr = prc.Complect.Complect == "Clones" || prc.Complect.Complect == "Archives"
                         ? NewStandardProvider(prc)
                         : NewProvider(prc);
            pr.Logger = project.App;
            pr.ProgressContext = project.Code;
            pr.Inf = inf;
            return pr;
        }

        //Запуск экземпляра провайдера через MEF, позднее связывание с dll
        [ImportMany(typeof(Provider))]
        private Lazy<Provider, IDictionary<string, object>>[] ImportProvs { get; set; }

        //Создать провайдер через MEF
        private Provider NewProvider(ProviderConfig prc)
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
        private Provider NewStandardProvider(ProviderConfig prc)
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