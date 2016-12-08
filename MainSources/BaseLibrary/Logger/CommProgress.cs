namespace BaseLibrary
{
    //Команда для отображения индикатора
    public class CommProgress : Comm
    {
        internal CommProgress(Logg logger, Comm parent, string text)
            : base(logger, parent, 0, 100)
        {
            SetText1(text);
        }

        //Отобразить на форме текст разного уровня
        public void SetText1(string text)
        {
            //ToDo отобразить на форме
        }
        public void SetText2(string text)
        {
            //ToDo отобразить на форме
        }
        public void SetText3(string text)
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
        public override Comm Finish(bool isBreaked = false)
        {
            base.Finish(isBreaked);
            //ToDo отобразить на форме
            return this;
        }
    }
}