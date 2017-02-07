using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Один выход (дисктретная, аналоговая или упакованная точка)
    internal class OutMir : SourceOut
    {
        internal OutMir(SourceBase source) : base(source) { }

        //Cигналы Unit и Indcation
        internal InitialSignal UnitSignal { get; set; }
        internal InitialSignal IndicationSignal { get; set; }
        //Id для получения значений из IZM_TII
        internal int IdChannel { get; set; }

        //Добавить к выходу сигнал, если такого еще не было
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            if (sig.Inf.Get("ValueType") == "Indication")
                return IndicationSignal = IndicationSignal ?? sig;
            return UnitSignal = UnitSignal ?? sig;
        }
        
        //Чтение значений по одному выходу из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("TIME");
            return AddMomReal(IndicationSignal, time, rec, "VALUE_INDICATION") +
                      AddMomReal(UnitSignal, time, rec, "VALUE_UNIT");
        }
    }
}