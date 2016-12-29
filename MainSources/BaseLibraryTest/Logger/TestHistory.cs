using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    //Одно событие тестовой истории
    internal class TestEvent
    {
        internal string Description { get; set; }
        internal string Params { get; set; }
        internal DateTime Time { get; set; }
        internal double FromStart { get; set; }
        internal string Status { get; set; }
    }

    //-------------------------------------------------------------------------------------------------------
    //CommandLog тестовой истории
    internal class TestCommandLog
    {
        internal string Command { get; set; }
        internal string Params { get; set; }
        internal string Status { get; set; }
        internal DateTime Time { get; set; }
        internal double ProcessLength { get; set; }
        internal string Context { get; set; }
        internal string Results { get; set; }

        internal List<TestEvent> Events = new List<TestEvent>();
    }

    //-------------------------------------------------------------------------------------------------------
    //CommandSuper тестовой истории
    internal class TestCommandSuper
    {
        internal string Command { get; set; }
        internal string Params { get; set; }
        internal DateTime BeginPeriod { get; set; }
        internal DateTime EndPeriod { get; set; }
        internal string ModePeriod { get; set; }
        internal string Status { get; set; }
        internal DateTime Time { get; set; }
        internal double ProcessLength { get; set; }
        internal string Results { get; set; }

        internal List<TestCommandLog> Logs = new List<TestCommandLog>();
    }

    //-------------------------------------------------------------------------------------------------------
    //Ошибка для ErrorsList тестовой истории
    internal class TestErrorLog
    {
        internal string Status { get; set; }
        internal string Description { get; set; }
        internal string Params { get; set; }
        internal DateTime Time { get; set; }
        internal string Command { get; set; }
        internal string Context { get; set; }
        internal DateTime BeginPeriod { get; set; }
        internal DateTime EndPeriod { get; set; }
    }

    //-------------------------------------------------------------------------------------------------------
    //Тестовая история
    internal class TestHistory : IHistory
    {
        public TestHistory(Logg logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logg Logger { get; private set; }

        //Списки команд Super и Log
        internal List<TestCommandSuper> Supers = new List<TestCommandSuper>();
        internal List<TestCommandLog> Logs = new List<TestCommandLog>();
        //Список ошибок
        internal List<TestErrorLog> Errors = new List<TestErrorLog>();
        //Текущие команды Super и Log
        internal TestCommandSuper CommandSuper { get; private set; }
        internal TestCommandLog CommandLog { get; private set; }

        public void WriteStartSuper(CommProgress command)
        {
            CommandSuper = new TestCommandSuper();
            Supers.Add(CommandSuper);
            CommandSuper.Command = command.Name;
            CommandSuper.Params = command.Params;
            CommandSuper.BeginPeriod = command.BeginPeriod;
            CommandSuper.EndPeriod = command.EndPeriod;
            CommandSuper.ModePeriod = command.ModePeriod;
            CommandSuper.Status = command.Status;
            CommandSuper.Time = command.StartTime;
        }

        public void WriteStart(CommLog command)
        {
            CommandLog = new TestCommandLog();
            Logs.Add(CommandLog);
            if (CommandSuper != null) CommandSuper.Logs.Add(CommandLog);
            CommandLog.Command = command.Name;
            CommandLog.Params = command.Params;
            CommandLog.Status = command.Status;
            CommandLog.Time = command.StartTime;
            CommandLog.Context = command.Context;
        }

        public void WriteFinishSuper(CommProgress command, string results)
        {
            CommandSuper.ProcessLength = command.FromStart;
            CommandSuper.Results = results;
            CommandSuper.Status = command.Status;
            CommandSuper = null;
        }

        public void WriteFinish(CommLog command, string results)
        {
            CommandLog.ProcessLength = command.FromStart;
            CommandLog.Results = results;
            CommandLog.Status = command.Status;
            CommandLog = null;
        }

        public void WriteEvent(string description, string pars)
        {
            if (CommandLog != null)
            {
                var ev = new TestEvent();
                CommandLog.Events.Add(ev);
                ev.Description = description;
                ev.Params = pars;
                ev.Time = DateTime.Now;
                ev.FromStart = Logger.CommandLog.FromStart;
                ev.Status = null;
            }
        }

        public void WriteError(ErrorCommand error)
        {
            if (CommandLog != null)
            {
                var ev = new TestEvent();
                CommandLog.Events.Add(ev);
                ev.Description = error.Text;
                ev.Params = error.ToLog();
                ev.Time = DateTime.Now;
                ev.FromStart = Logger.CommandLog.FromStart;
                ev.Status = error.Quality.ToRussian();
            }
        }

        public void WriteErrorToList(ErrorCommand error)
        {
            var err = new TestErrorLog();
            Errors.Add(err);
            err.Description = error.Text;
            err.Params = error.Params;
            err.Status = error.Quality.ToRussian();
            err.Time = DateTime.Now;
            err.Command = Logger.CommandLog.Name;
            err.Context = Logger.CommandLog.Context;
            if (Logger.CommandProgress != null && Logger.CommandProgress.BeginPeriod != Different.MinDate)
            {
                err.BeginPeriod = Logger.CommandProgress.BeginPeriod;
                err.EndPeriod = Logger.CommandProgress.EndPeriod;
            }
        }

        public void Close() { }
    }
}