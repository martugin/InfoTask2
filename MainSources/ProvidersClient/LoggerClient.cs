using System;
using BaseLibrary;

namespace ComClients
{
    //Оболочка логгера для вызова через COM
    public abstract class LoggerClient
    {
        protected LoggerClient()
        {
            Logger = new Logger();
        }

        //Логгер
        protected Logger Logger { get; private set; }

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

        //Запуск комманды для записи в History
        public void StartLog(string name,  //Имя комманды
                                       string pars = "",  //Дополнительная информация
                                       string context = "") //Контекст выполнения команды
        {
            Logger.StartLog(name, pars, CommandFlags.Simple, context);
        }

        //Запуск комманды для записи в SubHistory
        public void StartSubLog(string name,  //Имя комманды
                                             string pars = "")  //Дополнительная информация
        {
            Logger.StartSubLog(name, pars);
        }

        //Завершение комманды
        public void Finish(string results = null)
        {
            Logger.Finish(results);
        }

        //Запускает команду и возвращает строку с сообщением ошибки или ""
        protected string RunClientCommand(Action action, string errMess = "Ошибка выполнения команды")
        {
            using (Logger.Start(CommandFlags.Collect))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.AddError(errMess, ex);
                }
                return Logger.Command.ErrorMessage();
            }
        }
    }
}