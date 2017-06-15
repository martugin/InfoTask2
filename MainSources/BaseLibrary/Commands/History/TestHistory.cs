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
    //LogCommand тестовой истории
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
        internal DateTime PeriodBegin { get; set; }
        internal DateTime PeriodEnd { get; set; }
        internal string PeriodMode { get; set; }
        internal string Status { get; set; }
        internal DateTime Time { get; set; }
        internal double ProcessLength { get; set; }
        internal string Context { get; set; }
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
        internal DateTime PeriodBegin { get; set; }
        internal DateTime PeriodEnd { get; set; }
    }

    //-------------------------------------------------------------------------------------------------------
    //Тестовая история
    public class TestHistory : IHistory
    {
        public TestHistory(Logger logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; private set; }

        //Списки команд Super и Log
        internal List<TestCommandSuper> Supers = new List<TestCommandSuper>();
        internal List<TestCommandLog> Logs = new List<TestCommandLog>();
        //Список ошибок
        internal List<TestErrorLog> Errors = new List<TestErrorLog>();
        //Текущие команды Super и Log
        internal TestCommandSuper CommandSuper { get; private set; }
        internal TestCommandLog CommandLog { get; private set; }

        public void WriteStartSuper(ProgressCommand command)
        {
            CommandSuper = new TestCommandSuper();
            Supers.Add(CommandSuper);
            CommandSuper.Command = command.Name;
            CommandSuper.Params = command.Params;
            CommandSuper.PeriodBegin = Logger.PeriodBegin;
            CommandSuper.PeriodEnd = Logger.PeriodEnd;
            CommandSuper.PeriodMode = Logger.PeriodMode;
            CommandSuper.Status = command.Status;
            CommandSuper.Time = command.StartTime;
            CommandSuper.Context = command.Context;
        }

        public void WriteStart(LogCommand command)
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

        public void WriteFinishSuper(string results)
        {
            CommandSuper.ProcessLength = Logger.ProgressCommand.FromStart;
            CommandSuper.Results = results;
            CommandSuper.Status = Logger.ProgressCommand.Status;
            CommandSuper = null;
        }

        public void WriteFinish(string results)
        {
            CommandLog.ProcessLength = Logger.LogCommand.FromStart;
            CommandLog.Results = results;
            CommandLog.Status = Logger.LogCommand.Status;
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
                ev.FromStart = Logger.LogCommand.FromStart;
                ev.Status = null;
            }
        }

        public void WriteError(CommandError error)
        {
            if (CommandLog != null)
            {
                var ev = new TestEvent();
                CommandLog.Events.Add(ev);
                ev.Description = error.Text;
                ev.Params = error.ToLog();
                ev.Time = DateTime.Now;
                ev.FromStart = Logger.LogCommand.FromStart;
                ev.Status = error.Quality.ToRussian();
            }
        }

        public void WriteErrorToList(CommandError error)
        {
            var err = new TestErrorLog();
            Errors.Add(err);
            err.Description = error.Text;
            err.Params = error.Params;
            err.Status = error.Quality.ToRussian();
            err.Time = DateTime.Now;
            err.Command = Logger.LogCommand.Name;
            err.Context = Logger.LogCommand.Context;
            if (Logger.PeriodCommand != null)
            {
                err.PeriodBegin = Logger.PeriodBegin;
                err.PeriodEnd = Logger.PeriodEnd;
            }
        }

        public void ClearErrorsList()
        {
            Errors.Clear();
        }

        public void UpdateHistory() { }

        public void Close() { }
    }
}