namespace BaseLibrary
{
    //Команда для отображения текста 2-го уровня на форме индикатора
    public class CommandIndicatorText : Command
    {
        internal CommandIndicatorText(Logger logger, Command parent, double startProcent, double finishProcent, string text)
            : base(logger, parent, startProcent, finishProcent)
        {
            Logger.SetTabloText(2, text);
            Procent = 0;
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            Logger.SetTabloText(2, "");
            base.FinishCommand(isBreaked);
            Logger.CommandIndicatorText = null;
        }
    }
}