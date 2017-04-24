using System;

namespace BaseLibrary
{
    //Базовый класс для команд записи в лог
    public class LogBaseCommand : Command
    {
        protected internal LogBaseCommand(Logger logger, Command parent, double startProcent, double finishProcent, string name, string pars, string context) 
            : base(logger, parent, startProcent, finishProcent)
        {
            StartTime = DateTime.Now;
            Name = name;
            Params = pars;
            Context = context;
        }

        //Ссылка на историю
        protected IHistory History { get { return Logger.History; }}
        //Контекст выполнения 
        protected internal string Context { get; private set; }

        //Имя комманды
        protected internal string Name { get; private set; }
        //Параметры комманды
        protected internal string Params { get; private set; }

        //Время запуска комманды
        internal DateTime StartTime { get; private set; }
        //Разность времени между сейчас и стартом комманды
        internal double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }
    }
}