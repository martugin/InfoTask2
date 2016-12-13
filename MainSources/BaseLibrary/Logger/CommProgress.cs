namespace BaseLibrary
{
    //Команда для отображения индикатора
    public class CommProgress : Comm
    {
        internal CommProgress(Logg logger, Comm parent, string text0)
            : base(logger, parent, 0, 100)
        {
            Logger.ShowProcent = true;
            Logger.SetTabloText(0, text0);
        }

        //Отобразить индикатор
        public override double Procent
        {
            get { return Logger.TabloProcent; }
            set { Logger.TabloProcent = value; }
        }

        //Завершение команды
        protected override void FinishCommand(bool isBreaked)
        {
            Logger.ShowProcent = false;
            Logger.SetTabloText(0, "");
            base.FinishCommand(isBreaked);
        }
    }
}