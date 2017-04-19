using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Объект для фиктивных действий оператора
    internal class FictiveOperatorOut : ListSourceOut
    {
        internal FictiveOperatorOut(ListSource source) 
            : base(source) { }

        //Текст команды
        internal ListSignal TextSignal { get; private set; }
        //Номер команды
        internal ListSignal NumberSignal { get; private set; }

        protected override ListSignal AddSourceSignal(ListSignal sig)
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