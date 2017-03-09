using System;

namespace BaseLibrary
{
    //Команда для записи в History
    public class LogCommand : LogBaseCommand
    {
        protected internal LogCommand(Logger logger, Command parent, double startProcent, double finishProcent, string name, string context, string pars) 
            : base(logger, parent, startProcent, finishProcent, name, pars)
        {
            Context = context;
            Logger.SetTabloText(1, name + (context.IsEmpty() ? "" : (" (" + context + ")")));
            if (History != null)
                History.WriteStart(this);
        }
     
        //Контекст выполнения 
        protected internal string Context { get; private set; }
        //Результаты выполнения
        internal string Results { get; set; }

        //Добавить ошибку 
        public override void AddError(CommandError err)
        {
            if (History != null) 
                History.WriteError(err);
            base.AddError(err);
        }

        //Запуск операции, обрамляемой данной командой
        public override Command Run(Action action)
        {
            try
            {
                action();
            }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                AddError(new CommandError("Ошибка при обработке команды " + Name, ex));
            }
            return Finish();
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null)
            {
                if (isBreaked) History.WriteEvent("Прерывание команды", null);
                History.WriteFinish(Results);
            }
            Logger.SetTabloText(1, "");
            Logger.LogCommand = null;
        }
    }
}