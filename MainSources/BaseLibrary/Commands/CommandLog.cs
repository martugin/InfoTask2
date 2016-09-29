using System;

namespace BaseLibrary
{
    //Комманда для записи в History
    public class CommandLog : CommandLogBase
    {
        //name - имя команды, pars - параметры, все для записи в лог
        internal CommandLog(Logger logger, Command parent, double start, double finish, string name, string pars, CommandFlags flags, string context)
            : base(logger, parent, start, finish, name, pars, flags, context)
        {
            LogEventTime = DateTime.Now;
            var hist = Logger.History;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.AddNew();
                    if (Logger.CommandSubLog != null) 
                        hist.Put("SubHistoryId", Logger.CommandSubLog.HistoryId);
                    hist.Put("Command", Name);
                    hist.Put("Context", Context, true);
                    hist.Put("Params", Params);
                    hist.Put("Time", StartTime);
                    hist.Put("Status", Status);
                    Logger.LastHistoryId = HistoryId = hist.GetInt("HistoryId");
                });
        }

        internal CommandLog(Logger logger, Command parent, string name, string pars, CommandFlags flags, string context)
            : this(logger, parent, parent == null ? 0 : parent.Procent, parent == null ? 100 : parent.Procent, name, pars, flags, context)
        { }
        
        //Время логирования последнего события
        internal DateTime LogEventTime { private get; set; }
        //Разность времени между сейчас и временем предыдущего логирования
        public double FromEvent
        {
            get { return Math.Round(DateTime.Now.Subtract(LogEventTime).TotalSeconds, 2); }
        }

        //Завершение команды
        protected override void FinishCommand(string results)
        {
            var hist = Logger.History;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.MoveLast();
                    hist.Put("Status", Status);
                    hist.Put("Params", new[] { Params, results }.ToPropertyString());
                    hist.Put("ProcessLength", FromStart);
                });
            Logger.FinishLogCommand();
        }
    }
}
