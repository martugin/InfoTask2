using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Объект таблицы Values2
    internal class FictiveSmallOut : ListSourceOut
    {
        internal FictiveSmallOut(ListSource source) 
            : base(source) { }

        //Второй сигнал
        internal CloneSignal ValueSignal2 { get; private set; }

        //Id в таблице объектов
        internal int Id { get; set; }

        protected override CloneSignal AddInitialSignal(CloneSignal sig)
        {
            switch (sig.Inf["Signal"])
            {
                case "Value2":
                    return ValueSignal2 = ValueSignal2 ?? sig;
            }
            return ValueSignal = ValueSignal ?? sig;
        }

        //Чтение одной строчки значений
        protected override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            return AddMomReal(ValueSignal, time, rec, "ValueSignal") +
                      AddMomReal(ValueSignal2, time, rec, "ValueSignal2");
        }
    }
}