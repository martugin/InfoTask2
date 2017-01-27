using System;

namespace BaseLibrary
{
    //Команда для записи в History
    public class CommandLog : CommandLogBase
    {
        internal protected CommandLog(Logger logger, Command parent, double startProcent, double finishProcent, string name, string context, string pars) 
            : base(logger, parent, startProcent, finishProcent, name, pars)
        {
            Context = context;
            Logger.SetTabloText(1, name + (context.IsEmpty() ? "" : (" (" + context + ")")));
            if (History != null)
                History.WriteStart(this);
        }
     
        //Контекст выполнения 
        internal protected string Context { get; private set; }

        //Добавить ошибку 
        public override void AddError(ErrorCommand err)
        {
            if (History != null) 
                History.WriteError(err);
            base.AddError(err);
        }

        //Запуск операции, обрамляемой данной командой
        public override Command Run(Func<string> func)
        {
            string res = "";
            try
            {
                res = func();
            }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                AddError(new ErrorCommand("Ошибка при обработке команды " + Name, ex));
            }
            return Finish(res);
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            base.FinishCommand(results, isBreaked);
            if (History != null)
                History.WriteFinish(this, results);
            Logger.SetTabloText(1, "");
            Logger.CommandLog = null;
        }
    }
}