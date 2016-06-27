using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал 
    public class SourceInitSignal : SourceSignal
    {
        public SourceInitSignal(Source source, string code, DataType dataType, string signalInf)
            : base(source, code, dataType, signalInf)
        {
            BufMom = new MomEdit(dataType);
            PrevMom = new MomEdit(dataType);
            _beginMom = new MomEdit(dataType);
            _endMom = new MomEdit(dataType);
        }
        
        //Id в таблице сигналов клона
        internal int IdInClone { get; set; }

        //Значение среза на начало периода
        private readonly MomEdit _beginMom;
        //Значение среза для следующего периода
        private readonly MomEdit _endMom;
        //Буферное значение для добавления
        internal MomEdit BufMom { get; private set; }
        //Предыдущее значение, добавленное в клон
        internal MomEdit PrevMom { get; private set; }

        //Очистка списка значений
        internal void ClearMoments(bool clearBegin)
        {
            MList.Clear();
            _endMom.Time = Different.MinDate;
            if (clearBegin) _beginMom.Time = Different.MinDate;
        }

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal int AddMom(DateTime time, ErrMom err)  
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time <= Source.PeriodBegin && _beginMom.Time <= time)
                _beginMom.CopyAllFrom(BufMom);
            else if (time <= Source.PeriodEnd)
            {
                if (_endMom.Time <= time) _endMom.CopyAllFrom(BufMom);
                return PutMom(BufMom);
            }
            return 0;
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        internal int MakeBegin()
        {
            if (_beginMom.Time == Different.MinDate) return 0;
            return PutMom(_beginMom);
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        internal void MakeEnd()
        {
            if (_endMom.Time != Different.MinDate)
                _beginMom.CopyAllFrom(_endMom);
        }

        //Запись значения в список или клон
        private int PutMom(IMom mom)
        {
            if (IdInClone != 0) return PutClone(mom);
            MList.AddMom(mom);
            return 1;
        }

        //Запись значения в клон
        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        public int PutClone(IMom mom) //Рекордсет срезов клона
        {
            var rec = IsReal ? Source.CloneRec : Source.CloneStrRec;
            var recCut = IsReal ? Source.CloneCutRec : Source.CloneStrCutRec;
            int nwrite = 0;

            var d1 = Source.RemoveMinultes(mom.Time);
            var d = Source.RemoveMinultes(PrevMom.Time).AddMinutes(Source.CloneCutFrequency);
            while (d <= d1)
            {
                PutCloneRec(PrevMom, recCut, d);
                d = d.AddMinutes(Source.CloneCutFrequency);
                nwrite++;
            }
            PutCloneRec(mom, rec, mom.Time);
            PrevMom.CopyAllFrom(BufMom);
            return nwrite + 1;
        }

        private void PutCloneRec(IMom mom, RecDao rec, DateTime d)
        {
            rec.AddNew();
            rec.Put("SignalId", IdInClone);
            if (rec.Fields.ContainsKey("CutTime"))
                rec.Put("CutTime", d);
            rec.Put("Time", mom.Time);
            if (mom.Error != null)
                rec.Put("ErrNum", mom.Error.Number);
            if (IsReal) rec.Put("RealValue", mom.Real);
            else if (DataType == DataType.String)
                rec.Put("StrValue", mom.String);
            else if (DataType == DataType.Time)
                rec.Put("TimeValue", mom.Date);
            rec.Update();
        }
    }
}