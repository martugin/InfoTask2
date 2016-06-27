using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : SourceObject
    {
        internal ObjectOvation(OvationConn source, int id) : base(source)
        {
            Id = id;
        }
        
        //Сигнал со словом состояния
        internal SourceInitSignal StateSignal { get; set; }
        //Id в Historian
        internal int Id { get; private set; }
        
        //Добавить к объекту сигнал, если такого еще не было
        protected override SourceInitSignal AddNewSignal(SourceInitSignal sig)
        {
            if (sig.Inf["Prop"] == "STAT")
                return StateSignal = StateSignal ?? sig;
            return ValueSignal = ValueSignal ?? sig;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        public override int ReadMoments(IRecordRead rec)
        {
            _fValue = rec.GetDoubleNull("F_VALUE");
            _rawValue = rec.GetIntNull("RAW_VALUE");
            _sts = rec.GetIntNull("STS");
            var time = ReadTime(rec);
            double mean = (_fValue ?? _rawValue) ?? 0;
            int nwrite = 0;
            if (_sts != null)
                nwrite = AddMom(StateSignal, time, _sts);
            return nwrite + AddMom(ValueSignal, time, mean, ReadError(rec));
        }

        //Значения полей рекордсета
        private double? _fValue;
        private int? _rawValue;
        private int? _sts;
        
        //Формирование ошибки мгновенных значений по значению слова недостоверности
        private ErrMom ReadError(IRecordRead rec)
        {
            double? fMean = rec.GetDoubleNull("F_VALUE");
            int? rMean = rec.GetIntNull("RAW_VALUE");
            var sts = rec.GetIntNull("STS");
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (sts == null || (rMean == null && fMean == null))
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
            var time1 = rec.GetTime("TIMESTAMP");
            time1 = time1.AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0);
            DateTime time = time1.ToLocalTime();
            return time;
        }

        
    }
}