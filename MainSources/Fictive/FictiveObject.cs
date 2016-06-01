using CommonTypes;

namespace Fictive
{
    public class FictiveObject : SourceObject 
    {
        protected FictiveObject(string code) : base(code)
        {
        }

        //Сигнал недостоверности
        internal SourceSignal StateSignal { get; private set; }
        //Сигналы разного типа
        internal SourceSignal BoolSignal { get; private set; }
        internal SourceSignal IntSignal { get; private set; }
        internal SourceSignal RealSignal { get; private set; }
        internal SourceSignal TimeSignal { get; private set; }
        internal SourceSignal StringSignal { get; private set; }

        public override SourceSignal AddSignal(SourceSignal sig)
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

        //Для объекта определен срез
        public override bool HasBegin
        {
            get { return SignalsHasBegin(ValueSignal, StateSignal, BoolSignal, IntSignal, RealSignal, TimeSignal, StringSignal); }
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(ValueSignal, StateSignal, BoolSignal, IntSignal, RealSignal, TimeSignal, StringSignal);
        }
    }
}