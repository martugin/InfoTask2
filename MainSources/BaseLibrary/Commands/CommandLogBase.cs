using System;

namespace BaseLibrary
{
    //Базовый класс для комманд записи в SubHistory или History
    public abstract class CommandLogBase : Command
    {
        internal CommandLogBase(Logger logger, Command parent, double start, double finish, string name, string pars, CommandFlags flags, string context)
            : base(logger, parent, start, finish, flags, context)
        {
            StartTime = DateTime.Now;
            Name = name;
            Params = pars;
        }

        //Текущий HistoryId или SubHistoryId
        internal int HistoryId { get; set; }

        //Имя комманды
        public string Name { get; private set; }
        //Параметры комманды
        public string Params { get; private set; }

        //Время запуска комманды
        protected DateTime StartTime { get; private set; }
        //Разность времени между сейчас и стартом комманды
        protected double FromStart
        {
            get { return Math.Round(DateTime.Now.Subtract(StartTime).TotalSeconds, 2); }
        }

        //Строка для записи в лог состояния комманды
        protected string Status
        {
            get
            {
                if (IsBreaked) return "Прервано";
                if (!IsFinished) return "Запущено";
                return Quality.ToRussian();
            }
        }
    }
}