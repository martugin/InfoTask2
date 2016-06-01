using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectMir : SourceObject
    {
        internal ObjectMir(string code) 
            : base(code) {}

        //Cигналы Unit и Indcation
        internal SourceSignal UnitSignal { get; set; }
        internal SourceSignal IndicationSignal { get; set; }
        //Id для получения значений из IZM_TII
        public int IdChannel { get; set; }

        public override SourceSignal AddSignal(SourceSignal sig)
        {
            if (sig.Inf.Get("ValueType") == "Indication")
                return IndicationSignal = IndicationSignal ?? sig;
            return UnitSignal = UnitSignal ?? sig;
        }

        //Возвращает, есть ли у объекта неопределенные срезы
        public override bool HasBegin
        {
            get { return SignalsHasBegin(UnitSignal, IndicationSignal); }
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(UnitSignal, IndicationSignal);
        }
    }
}