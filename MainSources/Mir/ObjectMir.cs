using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectMir : SourceObject
    {
        public ObjectMir(SourceBase source, string context) 
            : base(source, context)
        { }

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
        
        protected override int AddObjectMoments()
        {
            return AddMom(IndicationSignal, ValueTime, _valueIndication) +
                      AddMom(UnitSignal, ValueTime, _valueUnit);
        }

        //Создание клона
        private double _valueIndication;
        private double _valueUnit;

        protected override DateTime ReadTime(IRecordRead rec)
        {
            return rec.GetTime("TIME");
        }
        protected override void ReadValue(IRecordRead rec)
        {
            _valueIndication = rec.GetDouble("VALUE_INDICATION");
            _valueUnit = rec.GetDouble("VALUE_UNIT");
        }
        protected override void PutValueToClone(IRecordAdd rec)
        {
            rec.Put("VALUE_INDICATION", _valueIndication);
            rec.Put("VALUE_UNIT", _valueUnit);
        }
    }
}