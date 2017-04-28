using System;
using System.Runtime.InteropServices;
using System.Threading;
using AppLibrary;
using CommonTypes;

namespace ComLaunchers
{
    //Интерфейс для ItLauncher
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IItLauncher
    {
        //Инициализация
        void Initialize(string appCode); //Код приложения
        //Закрытие клиента
        void Close();

        //Путь к каталогу InfoTask
        string InfoTaskDir { get; }
        //Имя организации-пользователя
        string UserOrg { get; }
        //Версия InfoTask
        string InfoTaskVersion { get; }
        //Дата версии InfoTask
        DateTime InfoTaskVersionDate { get; }
        
        //Код приложения
        string AppCode { get; }
        //Номер програмного продукта
        int ProductNumber{ get; }
        //Проверка активации приложения
        bool AppActivated { get; }

        //Загрузка проекта
        ILauncherProject LoadProject(string projectDir);

        //Переопределение команд логгера
        #region Logger
        //Ошибка и результат последней операции
        string ErrMess { get; }
        string ResultMess { get; }

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
        #endregion
    }

    //------------------------------------------------------------------------------------------------
    //Интерфейс событий для LauncherProject
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherEvents
    {
        [DispId(1)]
        void Finished();
    }

    //------------------------------------------------------------------------------------------------------------------------------
    //Клиент работы с функциями InfoTask, написанными на C#, вызываемыми из внешних приложений через COM
    [ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(ILauncherEvents))]
    public class ItLauncher :  IItLauncher
    {
        //Инициализация для нового приложения
        public void Initialize(string appCode) //Код приложения
        {
            _app = new App(appCode);
            _app.ExecutionFinished += OnExecutionFinished;
        }

        //Ссылка на приложение
        private App _app;

        //Закрытие клиента
        public void Close()
        {
            _app.ExecutionFinished -= OnExecutionFinished;
            _app.Dispose();
            _app = null;
            Thread.Sleep(100);
            GC.Collect();
            IsClosed = true;
        }
        //Клиент уже был закрыт
        protected internal bool IsClosed { get; private set; }

        //Путь к каталогу InfoTask
        public string InfoTaskDir { get { return ItStatic.InfoTaskDir(); } }

        //Имя организации-пользователя
        public string UserOrg
        {
            get { return ItStatic.UserOrg; }
        }
        //Версия InfoTask
        public string InfoTaskVersion
        {
            get { return ItStatic.InfoTaskVersion; }
        }
        //Дата версии InfoTask
        public DateTime InfoTaskVersionDate
        {
            get { return ItStatic.InfoTaskVersionDate; }
        }
        
        //Код приложения
        public string AppCode { get { return _app.Code; } }
        
        //Номер програмного продукта
        public int ProductNumber 
        { 
            get { return _app.ProductNumber; }
        }
        //Проверка активации приложения
        public bool AppActivated
        {
            get { return _app.IsActivated; }
        }

        //Загрузка проекта
        public ILauncherProject LoadProject(string projectDir) //Каталог проекта
        {
            return new LauncherProject(new AppProject(_app, projectDir));
        }



        //Работа с логгером
        #region Logger
        //Ошибка и результат последней операции
        public string ErrMess { get { return _app.CollectedError; } }
        public string ResultMess { get { return _app.CollectedResults; } }

        //Прервать выполнение
        public void Break()
        {
            _app.Break();
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
            _app.AddEvent(text, pars);
        }

        //Добавить предупреждение в историю
        public void AddWarning(string text, //Описание
                                            string pars = "") //Дополнительная информация
        {
            _app.AddWarning(text, null, pars);
        }

        //Добавить предупреждение в историю
        public void AddError(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            _app.AddError(text, null, pars);
        }

        //Установить процент текущей комманды
        public void SetProcent(double procent)
        {
            _app.Procent = procent;
        }

        //Запуск простой команды
        public void StartProcent(double startProcent, double finishProcent)
        {
            _app.Start(startProcent, finishProcent);
        }

        //Завершение комманды
        public void Finish(string results = null)
        {
            _app.Finish(results);
        }

        //Запуск команды для записи в History
        public void StartLog(string name,  //Имя команды
                                       string pars = "",  //Дополнительная информация
                                       string context = "") //Контекст выполнения команды
        {
            _app.StartLog(name, pars, context);
        }
        public void StartLogProcent(double startProcent, double finishProcent, string name, string pars = "", string context = "")
        {
            _app.StartLog(startProcent, finishProcent, name, pars, context);
        }
        //Завершение текущей команды логирования
        public void FinishLog(string results = null)
        {
            _app.FinishLog(results);
        }

        //Запуск команды, задающей период обработки
        public void StartPeriod(DateTime beg, DateTime en, string mode = "")
        {
            _app.StartPeriod(beg, en, mode);
        }
        //Завершение команды, задающей период обработки
        public void FinishPeriod()
        {
            _app.FinishPeriod();
        }

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "")  //Дополнительная информация
        {
            _app.StartProgress(name, pars);
        }

        //Звершение текущей команды отображения индикатора
        public void FinishProgress()
        {
            _app.FinishProgress();
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public void StartIndicatorText(string text)
        {
            _app.StartIndicatorText(text);
        }
        public void StartIndicatorTextProcent(double startProcent, double finishProcent, string text)
        {
            _app.StartIndicatorText(startProcent, finishProcent, text);
        }
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        public void FinishIndicatorText()
        {
            _app.FinishIndicatorText();
        }
        #endregion
    }
}
