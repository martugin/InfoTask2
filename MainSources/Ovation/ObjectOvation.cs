using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : SourceObject
    {
        internal ObjectOvation(OvationSource source, int id,  string context) 
            : base(source, context)
        {
            Id = id;
            Inf = "Id=" + Id;
        }
        
        //Сигнал со словом состояния
        internal SourceSignal StateSignal { get; set; }
        //Id в Historian
        internal int Id { get; private set; }
        
        //Добавить к объекту сигнал, если такого еще не было
        protected override SourceSignal AddNewSignal(SourceSignal sig)
        {
            if (sig.Inf["Prop"] == "STAT")
                return StateSignal = StateSignal ?? sig;
            return ValueSignal = ValueSignal ?? sig;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        protected override int AddObjectMoments()
        {
            var rMean = _fValue ?? _rawValue;
            return AddMom(StateSignal, ValueTime, _sts) +
                      AddMom(ValueSignal, ValueTime, rMean, ReadError());
        }

        //Формирование ошибки мгновенных значений по значению слова недостоверности
        private ErrMom ReadError()
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (_sts == null || (_fValue == null && _rawValue == null))
                return MakeError(4);//нет данных
            bool b8 = ((int)_sts).GetBit(8), b9 = ((int)_sts).GetBit(9);
            if (!b8 && !b9) return null;
            if (!b8) return MakeError(1);
            if (!b9) return MakeError(2);
            return MakeError(3);
        }

        //Поля значения объекта для клона
        private double? _fValue;
        private int? _rawValue;
        private int? _sts;

        //Чтение времени из рекордсета источника
        protected override DateTime ReadTime(IRecordRead rec)
        {
            var time1 = rec.GetTime("TIMESTAMP");
            time1 = time1.AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0);
            DateTime time = time1.ToLocalTime();
            return time;
        }

        //Чтение значений из источника для клона
        protected override void ReadValue(IRecordRead rec)
        {
            _fValue = rec.GetDouble("F_VALUE");
            _rawValue = rec.GetInt("RAW_VALUE");
            _sts = rec.GetInt("STS");
        }

        //Запись одной строчки значений из полей в клон
        protected override void PutValueToClone(IRecordAdd rec)
        {
            rec.Put("F_VALUE", _fValue);
            rec.Put("RAW_VALUE", _rawValue);
            rec.Put("STS", _sts);
        }
    }
}