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

        //Создание клона синхронно
        void MakeCloneSync(string cloneDir); //Каталог клона
        //Создание клона асинхронно
        void MakeCloneAsync(string cloneDir); //Каталог клона

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
            App = new App(appCode);
            App.ExecutionFinished += OnExecutionFinished;
        }

        //Ссылка на приложение
        protected App App { get; set; }

        //Закрытие клиента
        public void Close()
        {
            App.ExecutionFinished -= OnExecutionFinished;
            App.Dispose();
            App = null;
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
        public string AppCode { get { return App.Code; } }
        
        //Номер програмного продукта
        public int ProductNumber 
        { 
            get { return App.ProductNumber; }
        }
        //Проверка активации приложения
        public bool AppActivated
        {
            get { return App.IsActivated; }
        }

        //Загрузка проекта
        public ILauncherProject LoadProject(string projectDir) //Каталог проекта
        {
            return new LauncherProject(new AppProject(App, projectDir));
        }

        //Создание клона синхронно
        public void MakeCloneSync(string cloneDir) //Каталог клона
        {
            App.MakeCloneSync(cloneDir);
        }

        //Создание клона асинхронно
        public void MakeCloneAsync(string cloneDir) //Каталог клона
        {
            App.MakeCloneAsync(cloneDir);
        }

        //Работа с логгером
        #region Logger
        //Ошибка и результат последней операции
        public string ErrMess { get { return App.CollectedError; } }
        public string ResultMess { get { return App.CollectedResults; } }

        //Прервать выполнение
        public void Break()
        {
            App.Break();
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
            App.AddEvent(text, pars);
        }

        //Добавить предупреждение в историю
        public void AddWarning(string text, //Описание
                                            string pars = "") //Дополнительная информация
        {
            App.AddWarning(text, null, pars);
        }

        //Добавить предупреждение в историю
        public void AddError(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            App.AddError(text, null, pars);
        }

        //Установить процент текущей комманды
        public void SetProcent(double procent)
        {
            App.Procent = procent;
        }

        //Запуск простой команды
        public void StartProcent(double startProcent, double finishProcent)
        {
            App.Start(startProcent, finishProcent);
        }

        //Завершение комманды
        public void Finish(string results = null)
        {
            App.Finish(results);
        }

        //Запуск команды для записи в History
        public void StartLog(string name,  //Имя команды
                                       string pars = "",  //Дополнительная информация
                                       string context = "") //Контекст выполнения команды
        {
            App.StartLog(name, pars, context);
        }
        public void StartLogProcent(double startProcent, double finishProcent, string name, string pars = "", string context = "")
        {
            App.StartLog(startProcent, finishProcent, name, pars, context);
        }
        //Завершение текущей команды логирования
        public void FinishLog(string results = null)
        {
            App.FinishLog(results);
        }

        //Запуск команды, задающей период обработки
        public void StartPeriod(DateTime beg, DateTime en, string mode = "")
        {
            App.StartPeriod(beg, en, mode);
        }
        //Завершение команды, задающей период обработки
        public void FinishPeriod()
        {
            App.FinishPeriod();
        }

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "")  //Дополнительная информация
        {
            App.StartProgress(name, pars);
        }

        //Звершение текущей команды отображения индикатора
        public void FinishProgress()
        {
            App.FinishProgress();
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public void StartIndicatorText(string text)
        {
            App.StartIndicatorText(text);
        }
        public void StartIndicatorTextProcent(double startProcent, double finishProcent, string text)
        {
            App.StartIndicatorText(startProcent, finishProcent, text);
        }
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        public void FinishIndicatorText()
        {
            App.FinishIndicatorText();
        }
        #endregion
    }
}
