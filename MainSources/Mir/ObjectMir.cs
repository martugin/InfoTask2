using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectMir : SourceObject
    {
        public ObjectMir(SourceBase source) : base(source) { }

        //Cигналы Unit и Indcation
        internal InitialSignal UnitSignal { get; set; }
        internal InitialSignal IndicationSignal { get; set; }
        //Id для получения значений из IZM_TII
        public int IdChannel { get; set; }

        //Добавить к объекту сигнал, если такого еще не было
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            if (sig.Inf.Get("ValueType") == "Indication")
                return IndicationSignal = IndicationSignal ?? sig;
            return UnitSignal = UnitSignal ?? sig;
        }
        
        //Чтение значений по одному объекту из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("TIME");
            return AddMom(IndicationSignal, time, rec.GetDouble("VALUE_INDICATION")) +
                      AddMom(UnitSignal, time, rec.GetDouble("VALUE_UNIT"));
        }
    }
}