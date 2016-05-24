using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал 
    public class SourceSignal : ProviderSignal
    {
        public SourceSignal(string signalInf, string code, DataType dataType, ISource source, bool skipRepeats, int idInClone = 0) 
            : base(signalInf, code, dataType)
        {
            _source = (SourceBase)source;
            _idInClone = idInClone;
            _skipRepeats = skipRepeats;
            _bufMom = new MomEdit(dataType);
            if (idInClone != 0) _cloneMom = new MomEdit(dataType);
        }

        //Id в файле клона и т.п.
        private readonly int _idInClone;
        //Пропускать повторы значений
        private readonly bool _skipRepeats;
        //Источник
        private readonly SourceBase _source;

        //Список мгновенных значений
        public MomList MomList { get; set; }

        //Значение среза на начало периода
        private MomEdit _beginMom;
        //Буферное значение для добавления в список
        private readonly MomEdit _bufMom;
        //Предыдущее значение, записанное в клон
        private readonly MomEdit _cloneMom;

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        private int PutMom(DateTime time, ErrMom err)  
        {
            _bufMom.Time = time;
            _bufMom.Error = err;

            if (_beginMom == null)
                _beginMom = new MomEdit(_bufMom.DataType);
            if (_beginMom.Time <= _bufMom.Time && _bufMom.Time <= _source.PeriodBegin)
                _beginMom.CopyAllFrom(_bufMom);
            if (_source.IsCutReading) return 0;//Должен быть задан признак в источнике

            if (_source.CloneRec == null)
                return MomList.AddMom(_bufMom, _skipRepeats);
            return MomentToClone();
        }

        //Добавка мгновенных значений разного типа в список или клон, отдельные фйнкции для добавления среза
        public int AddMom(DateTime time, bool b, ErrMom err = null)
        {
            _bufMom.Boolean = b;
            return PutMom(time, err);
        }
        public int AddMom(DateTime time, int i, ErrMom err = null)
        {
            _bufMom.Integer = i;
            return PutMom(time, err);
        }
        public int AddMom(DateTime time, double r, ErrMom err = null)
        {
            _bufMom.Real = r;
            return PutMom(time, err);
        }
        public int AddMom(DateTime time, DateTime d, ErrMom err = null)
        {
            _bufMom.Date = d;
            return PutMom(time, err);
        }
        public int AddMom(DateTime time, string s, ErrMom err = null)
        {
            _bufMom.String = s;
            return PutMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        public int AddMom(DateTime time, object ob, ErrMom err = null)
        {
            _bufMom.Object= ob;
            return PutMom(time, err);
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        public int MakeBegin()
        {
            if (_beginMom == null) return 0;
            if (_source.CloneRec == null)
                return MomList.AddMom(_beginMom);
            return MomentToClone();
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        public int MakeEnd()
        {
            if (_source.CloneRec != null && _bufMom.Time != Different.MinDate && IsReal)
                return MomentToClone(true);
            return 0;
        }

        //Очищает значение среза
        public void ClearBegin()
        {
            _beginMom = null;
        }
        
        //Срез для сигнала определен
        public bool HasBegin { get { return _beginMom != null; } }

        //Добавляет мгновенное значение в клон, возвращает количество добавленных значений
        //Если withoutLast, то не добавляет само значение (только предыдущие раз в 10 минут), значения не реже чем раз в 10 минут
        private int MomentToClone(bool withoutLast = false)
        {
            if (!IsReal) return 0;
            int n = 0;
            if (_bufMom.Time != Different.MinDate)
                while (_bufMom.Time.Subtract(_cloneMom.Time).TotalMinutes > 10)
                {
                    _cloneMom.Time = _cloneMom.Time.AddMinutes(10);
                    ToClone();
                    n++;
                }
            if (!withoutLast && _cloneMom.Time < _bufMom.Time && (!_bufMom.ValueEquals(_cloneMom) || _bufMom.Error != _cloneMom.Error))
            {
                _cloneMom.CopyAllFrom(_bufMom);
                ToClone();
                n++;
            }
            return n;
        }

        //Запись в рекордсет клона
        private void ToClone()
        {
            var rec = _source.CloneRec;
            rec.AddNew();
            rec.Put("SignalId", _idInClone);
            rec.Put("Time", _cloneMom.Time);
            rec.Put("Value", _cloneMom.Real);
            if (_cloneMom.Error != null) rec.Put("NumError", _cloneMom.Error.ErrDescr.Number);
            rec.Update();
        }
    }
}