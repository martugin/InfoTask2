using System;

namespace BaseLibrary
{
    //Команда для записи в SuperHistory
    public class CommProgress : CommLogBase
    {
        //Конструктор с указанием периода обработки
        internal protected CommProgress(Logg logger, Comm parent, DateTime begin, DateTime end, string mode, string name, string pars)
            : base(logger, parent, 0, 100, name, pars)
        {
            BeginPeriod = begin;
            EndPeriod = end;
            ModePeriod = mode;
            Initialize();
        }
        //Конструктор с указанием текста 0-го уровня формы индикатора
        internal protected CommProgress(Logg logger, Comm parent, string text, string name, string pars)
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
        }
    }

    //--------------------------------------------------------------------------------------
    //Команда для отображения текста 2-го уровня на форме индикатора
    public class CommProgressText : Comm
    {
        internal CommProgressText(Logg logger, Comm parent, double startProcent, double finishProcent, string text)
            : base(logger, parent, startProcent, finishProcent)
        {
            Logger.SetTabloText(2, text);
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            Logger.SetTabloText(2, "");
            base.FinishCommand(results, isBreaked);
        }
    }
}