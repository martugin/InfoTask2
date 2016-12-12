using System;

namespace BaseLibrary
{
    //Базовый класс для команд записи в лог
    public class CommLogBase : Comm
    {
        internal protected CommLogBase(Logg logger, Comm parent, double startProcent, double finishProcent, string name, string pars) 
            : base(logger, parent, startProcent, finishProcent)
        {
            StartTime = DateTime.Now;
            Name = name;
            Params = pars;
        }

        //Ссылка на историю
        protected IHistory History { get { return Logger == null ? null : Logger.History; }}

        //Имя комманды
        internal protected string Name { get; private set; }
        //Параметры комманды
        internal protected string Params { get; private set; }

        //Время запуска комманды
        internal DateTime StartTime { get; private set; }
        //Разность времени между сейчас и стартом комманды
        internal double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------

    //Команда для записи в History
    public class CommLog : CommLogBase
    {
        internal protected CommLog(Logg logger, Comm parent, double startProcent, double finishProcent, string name, string context, string pars) 
            : base(logger, parent, startProcent, finishProcent, name, pars)
        {
            Context = context;
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
        public override Comm Run(Action action)
        {
            try
            {
                action();
            }
            catch (BreakException) { throw;}
            catch (Exception ex)
            {
                AddError(new ErrorCommand("Ошибка при обработке команды " + Name, ex));
            }
            return Finish();
        }

        //Завершение команды
        protected override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null) 
                History.WriteFinish(this);
            Logger.CommandLog = null;
        }
    }

    //--------------------------------------------------------------------------------------------------------

    //Команда для записи в SuperHistory
    public class CommSuperLog : CommLogBase
    {
        internal protected CommSuperLog(Logg logger, Comm parent, double startProcent, double finishProcent, DateTime begin, DateTime end, string mode, string name, string pars)
            : base(logger, parent, startProcent, finishProcent, name, pars)
        {
            BeginPeriod = begin;
            EndPeriod = end;
            ModePeriod = mode;
            if (History != null)
                History.WriteStartSuper(this);
        }
        
        //Период обработки
        public DateTime BeginPeriod { get; private set; }
        public DateTime EndPeriod { get; private set; }
        //Режим выполнения
        public string ModePeriod { get; private set; }

        //Завершение команды
        protected override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            if (History != null)
                History.WriteFinishSuper(this);
        }
    }
}