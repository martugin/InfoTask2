using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Generator;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для ItLauncher
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IItLauncher
    {
        //Инициализация
        void Initialize(string appCode, //Код приложения
                              string project); //Код проекта
        //Закрытие клиента
        void Close();

        //Ошибка и результат последней операции
        string ErrMess { get; }
        string ResultMess { get; }

        //Путь к каталогу InfoTask
        string InfoTaskDir { get; }

        //Генерация параметров
        void GenerateParams(string moduleDir);

        //Создание соединения
        SourConnect CreateSourConnect(string name, //Имя соединения
                                                         string complect); //Комплект
        ReceivConnect CreateReceivConnect(string name, //Имя соединения
                                                              string complect); //Комплект

        //Прервать выполнение
        void Break();

        //Добавить событие в историю
        void AddEvent(string text, //Описание
                              string pars = ""); //Дополнительная информация
        //Добавить предупреждение в историю
        void AddWarning(string text, //Описание
                                   string pars = ""); //Дополнительная информация
        //Добавить ошибку в историю
        void AddError(string text, //Описание
                               string pars = ""); //Дополнительная информация

        //Задать процент текущей команды
        void SetProcent(double procent);

        //Запуск простой комманды
        void StartProcent(double startProcent, double finishProcent); //Процент индикатора относительно команды родителя
        //Завершение текущей команды
        void Finish(string results = "");

        //Запуск команды логирования
        void StartLogProcent(double startProcent, double finishProcent, //Процент индикатора относительно команды родителя
                             string name, //Имя команды
                             string pars = "", //Дополнительная информация
                             string context = ""); //Контекст выполнения команды
        void StartLog(string name, string pars = "", string context = "");
        //Завершение текущей команды логирования
        void FinishLog(string results = null);

        //Запуск команды, задающей период обработки
        void StartPeriod(DateTime beg, DateTime en, string mode = "");
        //Завершение команды, задающей период обработки
        void FinishPeriod();

        //Запуск команды отображения индикатора
        void StartProgress(string name, //Имя команды
                                     string pars = ""); //Дополнительная информация
        //Звершение текущей команды отображения индикатора
        void FinishProgress();

        //Запуск команды отображения текста индикатора 2-ого уровня
        void StartIndicatorTextProcent(double startProcent, double finishProcent, string text);
        void StartIndicatorText(string text);
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        void FinishIndicatorText();
    }

    //------------------------------------------------------------------------------------------------

    //Интерфейс событий для LoggerClient
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILoggerClientEvents
    {
        [DispId(1)]
        void Finished();
    }

    //------------------------------------------------------------------------------------------------------------------------------

    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    [ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(ILoggerClientEvents))]
    public class ItLauncher :  IItLauncher
    {
        public ItLauncher()
        {
            Logger = new Logger {Indicator = new AppIndicator()};
            Logger.ExecutionFinished += OnExecutionFinished;
        }

        //Инициализация
        public void Initialize(string appCode, //Код приложения
                                        string project) //Код проекта
        {
            AppCode = appCode;
            Project = project;
            Logger.History = new AccessHistory(Logger, ItStatic.InfoTaskDir() + @"LocalData\History\" + appCode + @"\" + project + @"\History.accdb", ItStatic.HistoryTemplateFile);
        }

        //Закрытие клиента
        public void Close()
        {
            try
            {
                Logger.ExecutionFinished -= OnExecutionFinished;
                if (Logger.History != null)
                    Logger.History.Close();
            }
            catch { }
            Thread.Sleep(100);
            GC.Collect();
            IsClosed = true;
        }
        //Клиент уже был закрыт
        protected internal bool IsClosed { get; private set; }

        //Инициализация для запуска в тестах
        internal void InitializeTest(string appCode, string project)
        {
            AppCode = appCode;
            Project = project;
            Logger.History = new TestHistory(Logger);
        }

        //Ошибка и результат последней операции
        public string ErrMess { get { return Logger.CollectedError; } }
        public string ResultMess { get { return Logger.CollectedResults; } }

        //Путь к каталогу InfoTask
        public string InfoTaskDir { get { return ItStatic.InfoTaskDir(); } }

        //Код приложения
        protected internal string AppCode { get; private set; }
        //Код проекта
        protected internal string Project { get; private set; }

        //Генератор параметров, синглетон
        private TablGenerator _generator;

        //Генерация параметров
        public void GenerateParams(string moduleDir)
        {
            if (_generator == null)
                _generator = new TablGenerator(Logger);
            Logger.RunSyncCommand(() => _generator.GenerateParams(moduleDir));
        }

        //Создание соединения-источника
        public SourConnect CreateSourConnect(string name, string complect)
        {
            SourceConnect s = null;
            Logger.RunSyncCommand(() => { 
                s = (SourceConnect)Factory.CreateConnect(ProviderType.Source, name, complect, Logger);
            });
            return new RSourConnect(s, Factory);
        }

        //Создание соединения-приемника
        public ReceivConnect CreateReceivConnect(string name, string complect)
        {
            ReceiverConnect r = null;
            Logger.RunSyncCommand(() => {
                r = (ReceiverConnect)Factory.CreateConnect(ProviderType.Receiver, name, complect, Logger);
            });
            return new RReceivConnect(r, Factory);
        }

        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
        }

        //Работа с логгером
        #region Logger
        protected internal Logger Logger { get; private set; }

        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }

        //Событие, сообщающее внешнему приложению, что выполнение было прервано
        public delegate void EvDelegate();
        public event EvDelegate Finished;

        //Обработка события прерывания
        private void OnExecutionFinished(object sender, EventArgs e)
        {
            if (Finished != null) Finished();
        }
        
        //Добавить событие в историю
        public void AddEvent(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            Logger.AddEvent(text, pars);
        }

        //Добавить предупреждение в историю
        public void AddWarning(string text, //Описание
                                            string pars = "") //Дополнительная информация
        {
            Logger.AddWarning(text, null, pars);
        }

        //Добавить предупреждение в историю
        public void AddError(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            Logger.AddError(text, null, pars);
        }

        //Установить процент текущей комманды
        public void SetProcent(double procent)
        {
            Logger.Procent = procent;
        }

        //Запуск простой команды
        public void StartProcent(double startProcent, double finishProcent)
        {
            Logger.Start(startProcent, finishProcent);
        }

        //Завершение комманды
        public void Finish(string results = null)
        {
            Logger.Finish(results);
        }

        //Запуск команды для записи в History
        public void StartLog(string name,  //Имя команды
                                       string pars = "",  //Дополнительная информация
                                       string context = "") //Контекст выполнения команды
        {
            Logger.StartLog(name, pars, context);
        }
        public void StartLogProcent(double startProcent, double finishProcent, string name, string pars = "", string context = "")
        {
            Logger.StartLog(startProcent, finishProcent, name, pars, context);
        }
        //Завершение текущей команды логирования
        public void FinishLog(string results = null)
        {
            Logger.FinishLog(results);
        }

        //Запуск команды, задающей период обработки
        public void StartPeriod(DateTime beg, DateTime en, string mode = "")
        {
            Logger.StartPeriod(beg, en, mode);
        }
        //Завершение команды, задающей период обработки
        public void FinishPeriod()
        {
            Logger.FinishPeriod();
        }

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "")  //Дополнительная информация
        {
            Logger.StartProgress(name, pars);
        }

        //Звершение текущей команды отображения индикатора
        public void FinishProgress()
        {
            Logger.FinishProgress();
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public void StartIndicatorText(string text)
        {
            Logger.StartIndicatorText(text);
        }
        public void StartIndicatorTextProcent(double startProcent, double finishProcent, string text)
        {
            Logger.StartIndicatorText(startProcent, finishProcent, text);
        }
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        public void FinishIndicatorText()
        {
            Logger.FinishIndicatorText();
        }
        #endregion
    }
}
