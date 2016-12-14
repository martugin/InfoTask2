namespace BaseLibrary
{
    //Команда, обрамляющая опасную операцию
    public class CommDanger : Comm
    {
        internal CommDanger(Logg logger, Comm parent, double startProcent, double finishProcent) 
            : base(logger, parent, startProcent, finishProcent) { }
    }
}