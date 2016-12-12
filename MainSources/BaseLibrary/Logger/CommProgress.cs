namespace BaseLibrary
{
    //Команда для отображения индикатора
    public class CommProgress : Comm
    {
        internal CommProgress(Logg logger, Comm parent)
            : base(logger, parent, 0, 100)
        {
        }

        //Отобразить на форме текст, в табло уровня 1, 2 или 3 
        public CommProgress SetTabloText(int level, string text)
        {
            //ToDo отобразить на форме
        }
        

        //Отобразить индикатор
        public override double Procent
        {
            get { return base.Procent; }
            set 
            {
                base.Procent = value;
                //ToDo отобразить на форме
            }
        }

        //Завершение команды
        protected override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            //ToDo отобразить на форме
        }
    }
}