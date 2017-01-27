using System;

namespace BaseLibrary
{
    //Команда для отображения индикатора и записи в SuperHistory
    public class CommandProgress : CommandLogBase
    {
        //Конструктор с указанием периода обработки
        internal protected CommandProgress(Logger logger, Command parent, DateTime begin, DateTime end, string mode, string name, string pars)
            : base(logger, parent, 0, 100, name, pars)
        {
            BeginPeriod = begin;
            EndPeriod = end;
            ModePeriod = mode;
            var logg = Logger as LoggerTimed;
            if (logg != null)
            {
                logg.BeginPeriod = begin;
                logg.EndPeriod = end;
                logg.ModePeriod = mode;    
            }
            Initialize();
        }
        //Конструктор с указанием текста 0-го уровня формы индикатора
        internal protected CommandProgress(Logger logger, Command parent, string text, string name, string pars)
            : base(logger, parent, 0, 100, name, pars)
        {
            Logger.SetTabloText(0, text);
            Initialize();
        }
        private void Initialize()
        {
            Logger.ShowProcent = true;
            Logger.TabloProcent = 0;
            if (History != null)
                History.WriteStartSuper(this);
        }
        
        //Период обработки
        public DateTime BeginPeriod { get; private set; }
        public DateTime EndPeriod { get; private set; }
        //Режим выполнения
        public string ModePeriod { get; private set; }

        //Отобразить индикатор
        public override double Procent
        {
            get { return Logger.TabloProcent; }
            set { Logger.TabloProcent = value; }
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            base.FinishCommand(results, isBreaked);
            if (History != null)
                History.WriteFinishSuper(this, results);
            Logger.SetTabloText(0, "");
            Logger.ShowProcent = false;
            Logger.CommandProgress = null;
        }
    }

    //--------------------------------------------------------------------------------------
    //Команда для отображения текста 2-го уровня на форме индикатора
    public class CommandProgressText : Command
    {
        internal CommandProgressText(Logger logger, Command parent, double startProcent, double finishProcent, string text)
            : base(logger, parent, startProcent, finishProcent)
        {
            Logger.SetTabloText(2, text);
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            Logger.SetTabloText(2, "");
            base.FinishCommand(results, isBreaked);
            Logger.CommandProgressText = null;
        }
    }
}