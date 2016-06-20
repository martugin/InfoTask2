using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал 
    public class SourceSignal : ProviderSignal, ISourceSignal
    {
        public SourceSignal(ISource source, string code, DataType dataType, string signalInf, bool skipRepeats)
            : base(code, dataType, signalInf)
        {
            _source = source;
            _skipRepeats = skipRepeats;
            _momList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(_momList);
            BufMom = new MomEdit(dataType);
            _beginMom = new MomEdit(dataType);
            _endMom = new MomEdit(dataType);
            _cloneMom = new MomEdit(dataType);
        }

        //Пропускать повторы значений
        private readonly bool _skipRepeats;
        //Источник
        private readonly ISource _source;
        
        //Возвращаемый список значений
        private readonly MomList _momList;
        public IMomListReadOnly MomList { get; private set; }

        //Значение среза на начало периода
        private MomEdit _beginMom;
        //Значение среза для следующего периода
        private MomEdit _endMom;
        //Буферное значение для добавления в список
        internal MomEdit BufMom { get; private set; }
        //Предыдущее значение, записанное в клон
        private readonly MomEdit _cloneMom;

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal int PutMom(DateTime time, ErrMom err)  
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time <= _source.PeriodBegin && _beginMom.Time <= time)
                _beginMom.CopyAllFrom(BufMom);
            if (time <= _source.PeriodEnd && _endMom.Time <= time)
            {
                _endMom.CopyAllFrom(BufMom);
                return _momList.AddMom(BufMom, _skipRepeats);
            }

            if (_source.CloneRec == null)
                return _momList.AddMom(BufMom, _skipRepeats);
            return MomentToClone();
        }

        //Очистка списка значений
        internal void ClearMoments(bool clearBegin)
        {
            _momList.Clear();
            if (clearBegin) _beginMom.Time = Different.MinDate;
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        internal int MakeBegin()
        {
            if (_beginMom.Time == Different.MinDate) return 0;
            if (_source.CloneRec == null)
                return _momList.AddMom(_beginMom);
            return MomentToClone();
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        internal int MakeEnd()
        {
            if (_endMom.Time == Different.MinDate) return 0;
            if (_momList.Count == 0)
            _momList[MomList.Count - 1]
            if (_source.CloneRec != null && BufMom.Time != Different.MinDate)
                return MomentToClone(true);
            return 0;
        }

        //Добавляет мгновенное значение в клон, возвращает количество добавленных значений
        //Если withoutLast, то не добавляет само значение (только предыдущие раз в 10 минут), значения не реже чем раз в 10 минут
        private int MomentToClone(bool withoutLast = false)
        {
            if (!IsReal) return 0;
            int n = 0;
            if (BufMom.Time != Different.MinDate)
                while (BufMom.Time.Subtract(_cloneMom.Time).TotalMinutes > 10)
                {
                    _cloneMom.Time = _cloneMom.Time.AddMinutes(10);
                    ToClone();
                    n++;
                }
            if (!withoutLast && _cloneMom.Time < BufMom.Time && (!BufMom.ValueEquals(_cloneMom) || BufMom.Error != _cloneMom.Error))
            {
                _cloneMom.CopyAllFrom(BufMom);
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
            //rec.Put("SignalId", _idInClone);
            rec.Put("Time", _cloneMom.Time);
            rec.Put("Value", _cloneMom.Real);
            if (_cloneMom.Error != null) rec.Put("NumError", _cloneMom.Error.ErrDescr.Number);
            rec.Update();
        }
    }
}