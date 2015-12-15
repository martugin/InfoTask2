using System;

namespace BaseLibrary
{
    //Комманда для записи в SubHistory
    public class CommandSubLog : CommandLogBase
    {
        internal CommandSubLog(Logger logger, Command parent, double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags, string context)
            : base(logger, parent, start, finish, name, flags, context)
        {
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            _mode = mode;
            var hist = Logger.SubHistory;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.AddNew();
                    hist.Put("Command", Name);
                    hist.Put("Time", StartTime);
                    hist.Put("Status", Status);
                    hist.Put("PeriodBegin", PeriodBegin);
                    hist.Put("PeriodEnd", PeriodEnd);
                    hist.Put("Mode", _mode);
                    HistoryId = hist.GetInt("SubHistoryId");
                });
        }

        internal CommandSubLog(Logger logger, Command parent, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags, string context)
            : this(logger, parent, parent == null ? 0 : parent.Procent, parent == null ? 100 : parent.Procent, name, periodBegin, periodEnd, mode, flags, context)
        { }

        //Начло, конец и режим текущего расчета
        public DateTime PeriodBegin { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        private readonly string _mode;
        
        //Завершение команды
        protected override void FinishCommand(string results)
        {
            var hist = Logger.SubHistory;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.MoveLast();
                    hist.Put("Status", Status);
                    hist.Put("ProcessLength", FromStart);
                });
            Logger.FinishSubLogCommand();
        }
    }
}