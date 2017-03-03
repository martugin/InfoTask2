﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using BaseLibrary;
using CommonTypes;
using Generator;
using ProvidersLibrary;

namespace ComClients
{
    //Интерфейс для ItClient
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IItClient : ILoggerClient
    {
        //Инициализация
        void Initialize(string appCode, //Код приложения
                              string project); //Код проекта
        //Закрытие клиента
        void Close();

        //Генерация параметров
        string GenerateParams(string moduleDir);

        //Создание соединения
        SourConnect CreateSourConnect(string name, //Имя соединения
                                                        string complect); //Комплект
        ReceivConnect CreateReceivConnect(string name, //Имя соединения
                                                              string complect); //Комплект
    }

    //------------------------------------------------------------------------------------------------------------------------------

    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    [ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(ILoggerClientEvents))]
    public class ItClient : IndicatorClient , IItClient
    {
        public ItClient() : base(new Logger(), new Indicator())
        {
            SubscribeEvents();
        }

        //Инициализация
        public void Initialize(string appCode, //Код приложения
                                        string project) //Код проекта
        {
            AppCode = appCode;
            Project = project;
            Logger.History = new AccessHistory(Logger, DifferentIt.LocalDataProjectDir(project) + @"History\" + appCode + @"\History.accdb", DifferentIt.HistoryTemplateFile);
        }
        
        //Инициализация для запуска в тестах
        internal void InitializeTest()
        {
            AppCode = "Test";
            Project = "TestProject";
            Logger.History = new TestHistory(Logger);
        }

        //Закрытие клиента
        public void Close()
        {
            try
            {
                UnsubscribeEvents();
                if (Logger.History != null)
                    Logger.History.Close();
            }
            catch { }
            Thread.Sleep(100);
            GC.Collect();
            IsClosed = true;
        }
        //Клиент уже был закрыт
        protected bool IsClosed { get; private set; }

        //Код приложения
        protected string AppCode { get; private set; }
        //Код проекта
        protected string Project { get; private set; }
        
        //Генерация параметров
        public string GenerateParams(string moduleDir)
        {
            using (Logger.StartLog("Генерация параметров", moduleDir))
            {
                try
                {
                    var dir = moduleDir.EndsWith("\\") ? moduleDir : moduleDir + "\\";
                    var table = new GenTemplateTable("GenParams", "GenRule", "ErrMess", "CalcOn", "ParamId");
                    var subTable = new GenTemplateTable("GenSubParams", table, "GenRule", "ErrMess", "CalcOn", "SubParamId", "ParamId");
                    var dataTabls = new TablsList();
                    Logger.AddEvent("Загрузка структуры исходных таблиц", dir + "Tables.accdb");
                    using (var db = new DaoDb(dir + "Tables.accdb"))
                    {
                        dataTabls.AddDbStructs(db);
                        Logger.AddEvent("Загрузка значений из исходных таблиц");
                        dataTabls.LoadValues(db, true);
                    }
                    Logger.AddEvent("Загрузка и проверка генерирующих параметров");
                    var generator = new TablGenerator(Logger, dataTabls, dir + "CalcParams.accdb", table, subTable);
                    generator.Generate(dir + "Compiled.accdb", "GeneratedParams", "GeneratedSubParams");
                    Logger.AddEvent("Генерация завершена", generator.GenErrorsCount + " ошибок");
                    if (generator.GenErrorsCount == 0) return "";
                    return "Шаблон генерации содержит " + generator.GenErrorsCount + " ошибок";    
                }
                catch (Exception ex)
                {
                    Logger.AddError("Ошибка при генерации параметров", ex);
                    return ex.MessageString("Ошибка при генерации параметров");
                }
            }
        }

        //Создание соединения-источника
        public SourConnect CreateSourConnect(string name, string complect)
        {
            return new SourConnect(
                (SourceConnect)Factory.CreateConnect(ProviderType.Source, name, complect, Logger),
                Factory);
        }

        //Создание соединения-приемника
        public ReceivConnect CreateReceivConnect(string name, string complect)
        {
            return new ReceivConnect(
                (ReceiverConnect)Factory.CreateConnect(ProviderType.Receiver, name, complect, Logger),
                Factory);
        }

        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
        }
    }
}
