namespace BaseLibrary
{
    //Команда для отображения текста 2-го уровня на форме индикатора
    public class CommandProgressText : Command
    {
        internal CommandProgressText(Logger logger, Command parent, double startProcent, double finishProcent, string text)
            : base(logger, parent, startProcent, finishProcent)
        {
            Logger.SetTabloText(2, text);
            Procent = 0;
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