using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Generator;
using InfoTaskClient;
using ProvidersLibrary;

namespace ComClients
{
    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    public class InfoTaskClient : LoggerClient
    {
        //Код приложения
        protected string AppCode { get; private set; }
        //Код проекта
        protected string Project { get; private set; }

        //Инициализация
        public void Initialize(string appCode, //Код приложения
                                        string project) //Код проекта
        {
            AppCode = appCode;
            Project = project;
            Logger.History = new HistoryAccess(Logger, DifferentIt.LocalDataProjectDir(project) + @"History\" + appCode + @"\History.accdb", DifferentIt.HistoryTemplateFile);
        }


        public InfoTaskClient()
        {
            Logger.ShowIndicator += OnShowIndicator;
            Logger.ChangeProcent += OnChangeProcent;
            Logger.ChangeTabloText += OnChangeTabloText;
            if (Logger is LoggerTimed)
                ((LoggerTimed) Logger).ChangePeriod += OnChangePeriod;
        }

        //Клиент уже был закрыт
        protected bool IsClosed { get; private set; }

        //Закрытие клиента
        public void Close()
        {
            try { Logger.History.Close(); }
            catch { }
            Thread.Sleep(100);
            GC.Collect();
            IsClosed = true;
        }


        private delegate void FormDelegate();

        //Обработка события показать / скрыть форму индикатора
        private void OnShowIndicator(object sender, ShowIndicatorEventArgs e)
        {
            if (e.ShowIndicator) ShowIndicator();
            else HideIndicator();
        }

        //Обработка события изменения уровня индикатора
        private void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            
        }

        //Обработка события изменения текста на табло
        private void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
        {
            
        }

        //Обработка события изменения периода обработки
        private void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            if (_formTimed != null)
            {
                _formTimed.Invoke(new FormDelegate(() =>
                    {
                        _formTimed.PeriodBegin.Text = e.BeginPeriod.ToString();
                        _formTimed.PeriodEnd.Text = e.EndPeriod.ToString();
                    }));
            }
        }


        //Форма индикатора с указанием периода
        private IndicatorFormTimed _formTimed;
        //Форма индикатора с текстом
        private IndicatorFormTexted _formTexted;


        //Высвечивает форму
        private void ShowIndicator()
        {
            Application.EnableVisualStyles();
            Form form = null;
            if (Logger is LoggerTimed)
            {
                if (_formTimed == null)
                    _formTimed = new IndicatorFormTimed();
                form = _formTimed;
            }
            else
            {
                //Todo форма без указания периода
            }
            var screen = Screen.PrimaryScreen;
            form.Location = new Point(screen.WorkingArea.Width - form.Width - 1, screen.WorkingArea.Height - form.Height - 2);
            form.Show();
            form.Text = Project;
            form.Refresh();
        }

        //Спрятать форму индикатора
        private void HideIndicator()
        {
            Form form = Logger is LoggerTimed ? _formTimed : _formTimed;
            if (form != null) form.Invoke(new FormDelegate(() => _formTimed.Hide()));
        }
        
        //Фабрика провайдеров
        private ProvidersFactory _factory;
        protected ProvidersFactory Factory
        {
            get { return _factory ?? (_factory = new ProvidersFactory()); }
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
    }
}
