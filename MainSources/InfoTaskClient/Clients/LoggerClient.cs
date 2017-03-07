using System;
using System.Threading;
using BaseLibrary;

namespace ComClients
{
    //Оболочка логгера для вызова через COM
    public abstract class LoggerClient : ILoggerClient
    {
        protected LoggerClient()
        {
            Logger = new Logger {Indicator = new AppIndicator()};
            Logger.ExecutionFinished += OnExecutionFinished;
        }

        //Логгер
        protected Logger Logger { get; private set; }

        //Событие, сообщающее внешнему приложению, что выполнение было прервано
        public delegate void EvDelegate();
        public event EvDelegate Finished;

        //Обработка события прерывания
        private void OnExecutionFinished(object sender, EventArgs e)
        {
            if (Finished != null) Finished();
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
        protected bool IsClosed { get; private set; }

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
        public void Start(double startProcent, double finishProcent)
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
        public void StartLog(double startProcent, double finishProcent, string name, string pars = "", string context = "") 
        {
            Logger.StartLog(startProcent, finishProcent, name, pars, context);
        }
        //Завершение текущей команды логирования
        public void FinishLog(string results = null)
        {
            Logger.FinishLog(results);
        }

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "",  //Дополнительная информация
                                               string text = "") //Текст для отображения на индикаторе
        {
            Logger.StartProgress(text, name, pars);
        }
        public void StartProgress(string name,  //Имя команды
                                               string pars,  //Дополнительная информация
                                               DateTime beg, DateTime en, string mode = "") //Период расчета
        {
            Logger.StartProgress(beg, en, mode, name, pars);
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
        public void StartIndicatorText(double startProcent, double finishProcent, string text)
        {
            Logger.StartIndicatorText(startProcent, finishProcent, text);
        }
        //Завершение текущей команды отображения текста индикатора 2-ого уровня
        public void FinishIndicatorText()
        {
            Logger.FinishIndicatorText();
        }

        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }

        //Запускает команду и дожидается ее завершения
        protected void RunShortCommand(Action action)
        {
            Logger.StartCollect(false, true).Run(action);
        }
        //Запускает команду. Оповещение о завершении команды через событие Finished
        protected void RunLongCommand(Action action)
        {
            new Thread(() => Logger.StartCollect(false, true).Run(action)).Start();
        }
    }
}