using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Ovation
{
    //Один выход (дисктретная, аналоговая или упакованная точка)
    internal class OvationOut : ListSourceOut
    {
        internal OvationOut(OvationSource source, int id) : base(source)
        {
            Id = id;
        }
        
        //Сигнал со словом состояния
        internal InitialSignal StateSignal { get; set; }
        //Id в Historian
        internal int Id { get; private set; }
        
        //Добавить к объекту сигнал, если такого еще не было
        protected override InitialSignal AddInitialSignal(InitialSignal sig)
        {
            if (sig.Inf["Prop"].ToUpper() == "STAT")
                return StateSignal = StateSignal ?? sig;
            return ValueSignal = ValueSignal ?? sig;
        }

        //Чтение значений по одному выходу из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            double? fValue = rec.GetDoubleNull("F_VALUE");
            int? rawValue = rec.GetIntNull("RAW_VALUE");
            int? sts = rec.GetIntNull("STS");
            var time = ReadTime(rec);
            double mean = (fValue ?? rawValue) ?? 0;
            int nwrite = 0;
            if (sts != null)
                nwrite = AddMom(StateSignal, time, sts);
            return nwrite + AddMom(ValueSignal, time, mean, ReadError(fValue, rawValue, sts));
        }
        
        //Формирование ошибки мгновенных значений по значению слова недостоверности
        private MomErr ReadError(double? fValue, int? rawValue, int? sts)
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (sts == null || (rawValue == null && fValue == null))
                return MakeError(4);//нет данных
            bool b8 = ((int)sts).GetBit(8), b9 = ((int)sts).GetBit(9);
            if (!b8 && !b9) return null;
            if (!b8) return MakeError(1);
            if (!b9) return MakeError(2);
            return MakeError(3);
        }

        //Чтение времени из рекордсета источника
        private DateTime ReadTime(IRecordRead rec)
        {
            return rec.GetTime("TIMESTAMP")
                .AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0)
                    .ToLocalTime();
        }
    }
}