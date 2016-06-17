using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectMir : SourceObject
    {
        public ObjectMir(SourceBase source) : base(source)
        {
        }

        //Cигналы Unit и Indcation
        internal SourceSignal UnitSignal { get; set; }
        internal SourceSignal IndicationSignal { get; set; }
        //Id для получения значений из IZM_TII
        public int IdChannel { get; set; }

        protected override SourceSignal AddNewSignal(SourceSignal sig)
        {
            if (sig.Inf.Get("ValueType") == "Indication")
                return IndicationSignal = IndicationSignal ?? sig;
            return UnitSignal = UnitSignal ?? sig;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        public override int ReadValueFromRec(IRecordRead rec)
        {
            DateTime time = rec.GetTime("TIME");
            return AddMom(IndicationSignal, time, rec.GetDouble("VALUE_INDICATION")) +
                      AddMom(UnitSignal, time, rec.GetDouble("VALUE_UNIT"));
        }
    }
}