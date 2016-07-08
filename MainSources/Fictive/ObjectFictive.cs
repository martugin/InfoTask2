using System;
using ProvidersLibrary;

namespace Fictive
{
    public class ObjectFictive : SourceObject 
    {
        //Фиктивный объект
        //Сигналы задаются свойствами ValuesInterval - частота возвращаемых значений в секундах
        public ObjectFictive(FictiveSource source, int valuesInterval) : base(source)
        {
            ValuesInterval = valuesInterval;
        }

        //Сигнал недостоверности
        internal InitialSignal StateSignal { get; private set; }
        //Сигналы разного типа
        internal InitialSignal BoolSignal { get; private set; }
        internal InitialSignal IntSignal { get; private set; }
        internal InitialSignal RealSignal { get; private set; }
        internal InitialSignal TimeSignal { get; private set; }
        internal InitialSignal StringSignal { get; private set; }

        //Частота возвращаемых значений в секундах
        internal int ValuesInterval { get; private set; }
        //Объект инициализирован
        internal bool IsInitialized { get; set; }

        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            switch (sig.Inf["Signal"])
            {
                case "State":
                    return StateSignal = StateSignal ?? sig;
                case "Bool":
                    return BoolSignal = BoolSignal ?? sig;
                case "Int":
                    return IntSignal = IntSignal ?? sig;
                case "Real":
                    return RealSignal = RealSignal ?? sig;
                case "Time":
                    return TimeSignal = TimeSignal ?? sig;
                case "String":
                    return StringSignal = StringSignal ?? sig;
            }
            return ValueSignal = ValueSignal ?? sig;
        }

        //Заполняет значения сигналов равномерным списком значений
        internal int MakeUniformValues(DateTime beg, DateTime en, bool isCut)
        {
            int n = 0;
            int nwrite = 0;
            DateTime t = beg;
            if (!isCut) t = t.AddMilliseconds(ValuesInterval);
            while (t <= en)
            {
                nwrite += AddMom(StateSignal, t, 10);
                nwrite += AddMom(BoolSignal, t, n%2 == 1);
                nwrite += AddMom(IntSignal, t, n);
                nwrite += AddMom(ValueSignal, t, n, MakeError(n % 3));
                nwrite += AddMom(RealSignal, t, n / 2.0);
                nwrite += AddMom(StringSignal, t, "ss" + n);
                nwrite += AddMom(TimeSignal, t, t.AddSeconds(1));
                t = t.AddMilliseconds(ValuesInterval);
                n++;
            }
            return nwrite;
        }
    }
}