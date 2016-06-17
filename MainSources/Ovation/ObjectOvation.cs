using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : SourceObject
    {
        internal ObjectOvation(OvationSource source, int id, string code) : base(source)
        {
            Id = id;
            Inf = code.Substring(0, code.IndexOf('.')) + "; Id=" + Id;
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
        public override int ReadValueFromRec(IRecordRead rec)
        {
            var time1 = rec.GetTime("TIMESTAMP");
            time1 = time1.AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0);
            DateTime time = time1.ToLocalTime();
            var rMean = rec.GetDouble("F_VALUE", rec.GetInt("RAW_VALUE"));

            return AddMom(StateSignal, time, rec.GetInt("STS")) +
                      AddMom(ValueSignal, time, rMean, MakeError(rec));
        }

        //Формирование ошибки мгновенных значений по значению слова недостоверности
        private ErrMom MakeError(IRecordRead rec)
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (rec.IsNull("STS") || (rec.IsNull("F_VALUE") && rec.IsNull("RAW_VALUE")))
                return MakeError(4);//нет данных
            int state = rec.GetInt("STS");
            bool b8 = state.GetBit(8), b9 = state.GetBit(9);
            if (!b8 && !b9) return null;
            if (!b8) return MakeError(1);
            if (!b9) return MakeError(2);
            return MakeError(3);
        }
    }
}