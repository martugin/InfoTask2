using System;
using System.Threading;
using BaseLibrary;

namespace ComClients
{
    //Оболочка логгера для вызова через COM
    public abstract class LoggerClient
    {
        //Логгер
        internal protected Logger Logger { get; protected set; }
        
        //Добавить событие в историю
        public void AddEvent(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            Logger.AddEvent(text, pars);
        }

        //Добавить предупреждение в историю
        public void AddWarning(string text, //Описание
                                            string par = "") //Дополнительная информация
        {
            Logger.AddWarning(text, null, par);
        }

        //Добавить предупреждение в историю
        public void AddError(string text, //Описание
                                        string par = "") //Дополнительная информация
        {
            Logger.AddError(text, null, par);
        }

        //Запуск простой команды
        public void Start()
        {
            Logger.Start();
        }
        public void Start(double startProcent, double finishProcent)
        {
            Logger.Start(startProcent, finishProcent);
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

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "",  //Дополнительная информация
                                               string text = "") //Текст для отображения на индикаторе
        {
            Logger.StartProgress(text, name, pars);
        }
        public void StartProgress(string name,  //Имя команды
                                               string pars,  //Дополнительная информация
                                               DateTime beg, DateTime en) //Период расчета
        {
            Logger.StartProgress(beg, en, name, pars);
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

        //Завершение комманды
        public void Finish(string results = null)
        {
            Logger.Finish(results);
        }

        //Установить процент текущей комманды
        public void SetProcent(double procent)
        {
            Logger.Procent = procent;
        }

        //Запускает команду и дожидается ее завершения
        protected void RunShortCommand(Action action)
        {
            Logger.StartCollect(false, true).Run(action);
        }
        //Запускает команду. Оповещение о завершении команды через событие
        protected void RunLongCommand(Action action)
        {
            new Thread(() => Logger.StartCollect(false, true).Run(action)).Start();
        }

        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }
    }
}