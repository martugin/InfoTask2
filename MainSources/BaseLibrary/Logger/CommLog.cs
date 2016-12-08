using System;

namespace BaseLibrary
{
    //Базовый класс для команд записи в лог
    public class CommLog : Comm
    {
        public CommLog(Logg logger, Comm parent, double startProcent, double finishProcent, string name, string context, string pars) 
            : base(logger, parent, startProcent, finishProcent)
        {
            StartTime = DateTime.Now;
            Name = name;
            Params = pars;
        }

        //Имя комманды
        internal string Name { get; private set; }
        //Параметры комманды
        internal string Params { get; private set; }
        //Контекст выполнения 
        internal string Context { get; private set; }

        //Время запуска комманды
        internal DateTime StartTime { get; private set; }
        //Разность времени между сейчас и стартом комманды
        internal double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }

        //Строка для записи в лог состояния комманды
        internal string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                return Quality.ToRussian();
            }
        }

        //Завершение команды
        public override Comm Finish(bool isBreaked = false)
        {
            base.Finish(isBreaked);
            if (Logger.CommandLog == this)
                Logger.History.WriteFinish(this);
            if (Logger.CommandSubLog == this)
                Logger.History.WriteFinishSub(this);
            return this;
        }
    }
}