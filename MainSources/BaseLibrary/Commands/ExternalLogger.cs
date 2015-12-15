using System;

namespace BaseLibrary
{
    //Класс для переопределения операция логгера
    public abstract class ExternalLogger : IContextable
    {
        protected ExternalLogger(Logger logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; set; }

        //Контекст заданный по умолчанию
        public abstract string Context { get; }

        public double Procent
        {
            get { return Logger.Procent; }
            set { Logger.Procent = value; }
        }

        public void AddEvent(string description, double procent)
        {
            Logger.AddEvent(description, procent);
        }
        public void AddEvent(string description, string pars = "")
        {
            Logger.AddEvent(description, pars);
        }
        public void AddEvent(string description, string pars, double procent)
        {
            Logger.AddEvent(description, pars, procent);
        }

        public void AddError(string text, Exception ex = null, string pars = "", string context = "")
        {
            Logger.AddError(text, ex, pars, context.IsEmpty() ? Context : context);
        }
        public void AddWarning(string text, Exception ex = null, string pars = "", string context = "")
        {
            Logger.AddWarning(text, ex, pars, context.IsEmpty() ? Context : context);
        }

        public Command Command { get { return Logger.Command; } }

        public Command Start(double start, double finish, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.Start(start, finish, flags, context);
        }
        public Command Start(CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.Start(flags, context);
        }

        public CommandLog StartLog(double start, double finish, string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.StartLog(start, finish, name, pars, flags, context);
        }
        public CommandLog StartLog(string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.StartLog(name, pars, flags, context);
        }

        public CommandSubLog StartSubLog(double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.StartSubLog(start, finish, name, periodBegin, periodEnd, mode, flags, context);
        }
        public CommandSubLog StartSubLog(string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Logger.StartSubLog(name, periodBegin, periodEnd, mode, flags, context);
        }

        public bool Start(Action action, double start, double finish, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.Start(action, start, finish, flags, errMess);
        }
        public bool Start(Action action, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.Start(action, flags, errMess);
        }

        public bool StartLog(Action action, double start, double finish, string name, string pars = "", string context = "", CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.StartLog(action, start, finish, name, pars, flags, context, errMess);
        }
        public bool StartLog(Action action, string name, string pars = "", string context = "", CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.StartLog(action, name, pars, flags, context, errMess);
        }

        public bool StartSubLog(Action action, double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.StartSubLog(action, start, finish, name, periodBegin, periodEnd, mode, flags, errMess);
        }
        public bool StartSubLog(Action action, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return Logger.StartSubLog(action, name, periodBegin, periodEnd, mode, flags, errMess);
        }

        public Command Finish(string results = null, bool isBreaked = false)
        {
            return Logger.Finish(results, isBreaked);
        }
        
        public bool Danger(Func<bool> operation, int repetitions, int errorWaiting = 0, string errMess = "Не удалось выполнить опасную операцию", Func<bool> errorOperation = null)
        {
            return Logger.Danger(operation, repetitions, errorWaiting, errMess, errorOperation);
        }
    }
}