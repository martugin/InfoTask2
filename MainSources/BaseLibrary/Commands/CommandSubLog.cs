
namespace BaseLibrary
{
    //Комманда для записи в SubHistory
    public class CommandSubLog : CommandLogBase
    {
        internal CommandSubLog(Logger logger, Command parent, double start, double finish, string name, string pars, CommandFlags flags, string context)
            : base(logger, parent, start, finish, name, pars, flags, context)
        {
            var hist = Logger.SubHistory;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.AddNew();
                    hist.Put("Command", Name);
                    hist.Put("Time", StartTime);
                    hist.Put("Status", Status);
                    hist.Put("Params", Params);
                    HistoryId = hist.GetInt("SubHistoryId");
                });
        }

        internal CommandSubLog(Logger logger, Command parent, string name, string pars, CommandFlags flags, string context)
            : this(logger, parent, parent == null ? 0 : parent.Procent, parent == null ? 100 : parent.Procent, name, pars, flags, context)
        { }
        
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