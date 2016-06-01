using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : SourceObject
    {
        internal ObjectOvation(int id, string code) : base(code)
        {
            Id = id;
            Inf = code.Substring(0, code.IndexOf('.')) + "; Id=" + Id;
        }

        //Сигнал со словом состояния
        internal SourceSignal StateSignal { get; set; }
        //Id в Historian
        internal int Id { get; private set; }

        public override SourceSignal AddSignal(SourceSignal sig)
        {
            if (sig.Inf["Prop"] == "STAT")
                return StateSignal = StateSignal ?? sig;
            return ValueSignal = ValueSignal ?? sig;
        }

        //Возвращает, есть ли у объекта неопределенные срезы
        public override bool HasBegin
        {
            get { return SignalsHasBegin(ValueSignal, StateSignal); }
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(ValueSignal, StateSignal);
        }
    }
}