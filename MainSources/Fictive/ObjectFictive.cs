using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    public class ObjectFictive : SourceObject 
    {
        //Фиктивный объект
        //Сигналы задаются свойствами ValuesInterval - частота возвращаемых значений в секундах
        public ObjectFictive(SourceBase source, int valuesInterval) : base(source)
        {
            ValuesInterval = valuesInterval;
        }
        public ObjectFictive(SourceBase source) 
            : base(source) { }

        //Сигнал недостоверности
        internal UniformSignal StateSignal { get; private set; }
        //Сигналы разного типа
        internal UniformSignal BoolSignal { get; private set; }
        internal UniformSignal IntSignal { get; private set; }
        internal UniformSignal RealSignal { get; private set; }
        internal UniformSignal TimeSignal { get; private set; }
        internal UniformSignal StringSignal { get; private set; }

        //Частота возвращаемых значений в секундах
        internal int ValuesInterval { get; private set; }
        //Объект инициализирован
        internal bool IsInitialized { get; set; }

        //Id в таблице объектов
        internal int Id { get; set; }

        protected override UniformSignal AddNewSignal(UniformSignal sig)
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
            int n = isCut ? 0 : 1;
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

        //Чтение одной строчки значений
        public override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            var state = rec.GetInt("StateSignal");
            return AddMom(ValueSignal, time, rec.GetDouble("ValueSignal")) +
                   AddMom(StateSignal, time, state) +
                   AddMom(BoolSignal, time, rec.GetBool("BoolSignal")) +
                   AddMom(IntSignal, time, rec.GetInt("IntSignal")) +
                   AddMom(RealSignal, time, rec.GetDouble("RealSignal"), Source.MakeError(state, this)) +
                   AddMom(StringSignal, time, rec.GetString("StringSignal"), Source.MakeError(state, this)) +
                   AddMom(TimeSignal, time, rec.GetTime("TimeSignal"));
        }
    }
} 