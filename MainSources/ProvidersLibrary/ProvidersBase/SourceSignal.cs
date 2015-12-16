using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал 
    public class SourceSignal : ProviderSignal
    {
        public SourceSignal(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0) 
            : base(signalInf, code, dataType, provider)
        {
            _idInClone = idInClone;
        }
        
        //Id в файле клона и т.п.
        private readonly int _idInClone;
        
        //Значение среза на начало периода
        public IMom BeginMom { get; private set; }

        //Список мгновенных значений
        public MomList Value { get; set; }

        //Добавка мгновенных значений в список или клон
        //Возвращает количество реально добавленных значений
        //rec - рекордсет клона, если не задан, то добавляется в список
        //если forBegin, то значение не пишется в список сразу, т.к. оно предназначено для формирования среза
        public int AddMom(IMom mom, bool forBegin = false, bool skipEquals = true, bool add10Min = true)
        {
            if (forBegin) ChangeBegin(mom);
            else
            {
                var rec = ((SourceBase)Provider).CloneRec;
                if (rec == null)
                {
                    ChangeBegin(mom);
                    return Value.AddMom(mom, skipEquals) == null ? 0 : 1;
                }
                if (IsReal) return MomentToClone(rec, mom.Time, mom.Real, mom.Error, false, add10Min);    
            }
            return 0;
        }
        //Добавка мгновенных значений разного типа в список или клон
        public int AddMom(DateTime time, bool b, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(new MomBool(time, b, err), forBegin);
        }
        public int AddMom(DateTime time, int i, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(new MomInt(time, i, err), forBegin);
        }
        public int AddMom(DateTime time, double r, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(new MomReal(time, r, err), forBegin);
        }
        public int AddMom(DateTime time, DateTime d, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(new MomTime(time, d, err), forBegin);
        }
        public int AddMom(DateTime time, string s, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(new MomString(time, s, err), forBegin);
        }
        //Добавка мгновенных значений с указанием типов данных, значение берется из типа object
        public int AddMom(DataType dtype, DateTime time, object ob, ErrMom err = null, bool forBegin = false)
        {
            return AddMom(MomFactory.NewMom(dtype, time, ob, err), forBegin);
        }

        //Присваевает новое значение в BeginMom
        private void ChangeBegin(IMom mv)
        {
            if (BeginMom == null || BeginMom.Time < mv.Time)
                BeginMom = mv;
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        public int MakeBegin(DateTime beginTime)
        {
            if (BeginMom == null) return 0;
            var rec = ((SourceBase)Provider).CloneRec;
            if (rec == null)
            {
                Value.AddMom(BeginMom.Clone(beginTime));
                return 1;
            }
            if (IsReal) return MomentToClone(rec, beginTime, BeginMom.Real, BeginMom.Error);
            return 0;
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        public int MakeEnd(DateTime endTime)
        {
            if (BeginMom != null) BeginMom = BeginMom.Clone(endTime);
            var rec = ((SourceBase)Provider).CloneRec;
            if (rec != null && _time != Different.MinDate && IsReal)
                return MomentToClone(rec, endTime, _val, _err, true);
            return 0;
        }

        //Для сигнала опредлено значение среза на время time
        public bool BeginEquals(DateTime t)
        {
            if (BeginMom == null) return false;
            return BeginMom.Time.EqulasToSeconds(t); 
        }

        //Текущие время, значение и недостоверность записи в клон
        private DateTime _time = Different.MinDate;
        private double _val = Double.NaN;
        private ErrMom _err;

        //Добавляет мгновенное значение в клон, возвращает количество добавленных значений
        //Если withoutLast, то не добавляет само значение (только предыдущие раз в 10 минут), если add10Min, то добавляет значения не реже чем раз в 10 минут
        private int MomentToClone(RecDao rec, DateTime time, double val, ErrMom err, bool withoutLast = false, bool add10Min = true)
        {
            int n = 0;
            if (_time != Different.MinDate && add10Min)
                while (time.Subtract(_time).TotalMinutes > 10)
                {
                    _time = _time.AddMinutes(10);
                    ToClone(rec);
                    n++;
                }
            if (_time < time && (val != _val || err != _err) && !withoutLast)
            {
                _time = time;
                _val = val;
                _err = err;
                ToClone(rec);
                n++;
            }
            return n;
        }

        //Запись в рекордсет клона
        private void ToClone(RecDao rec)
        {
            rec.AddNew();
            rec.Put("SignalId", _idInClone);
            rec.Put("Time", _time);
            rec.Put("Value", _val);
            if (_err != null) rec.Put("NumError", _err.ErrDescr.Number);
            rec.Update();
        }
    }
}