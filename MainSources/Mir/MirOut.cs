﻿using BaseLibrary;
using ProvidersLibrary;

namespace Mir
{
    //Один выход (дисктретная, аналоговая или упакованная точка)
    internal class MirOut : ListSourceOut
    {
        internal MirOut(ListSource source) : base(source) { }

        //Cигналы Unit и Indcation
        internal ListSignal UnitSignal { get; set; }
        internal ListSignal IndicationSignal { get; set; }
        //Id для получения значений из IZM_TII
        internal int IdChannel { get; set; }

        //Добавить к выходу сигнал, если такого еще не было
        protected override ListSignal AddSourceSignal(ListSignal sig)
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