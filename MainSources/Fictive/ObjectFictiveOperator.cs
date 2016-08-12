using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Объект для фиктивных действий оператора
    internal class ObjectFictiveOperator : SourceObject
    {
        internal ObjectFictiveOperator(SourceBase source) 
            : base(source) { }

        //Текст команды
        internal InitialSignal TextSignal { get; private set; }
        //Номер команды
        internal InitialSignal NumberSignal { get; private set; }

        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            switch (sig.Inf["Signal"])
            {
                case "CommandText":
                    return TextSignal = TextSignal ?? sig;
                case "CommandNumber":
                    return NumberSignal = NumberSignal ?? sig;
            }
            return null;
        }

        //Чтение одной строчки значений
        protected override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            return AddMomString(TextSignal, time, rec, "CommandText") +
                      AddMomInt(NumberSignal, time, rec, "CommandNumber");
        } 
    }
}