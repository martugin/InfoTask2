using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал, значение которого считывается из источника, с работой со срезами
    public class UniformSignal : InitialSignal
    {
        public UniformSignal(SourceConnect connect, string code, string codeObject, DataType dataType, string signalInf)
            : base(connect, code, codeObject, dataType, signalInf)
        {
            PrevMom = new MomEdit(dataType);
            _beginMom = new MomEdit(dataType);
            _endMom = new MomEdit(dataType);    
        }

        //Значение среза на начало периода
        private readonly MomEdit _beginMom;
        //Значение среза для следующего периода
        private readonly MomEdit _endMom;
        //Предыдущее значение, добавленное в клон
        internal MomEdit PrevMom { get; private set; }

        //Очистка списка значений
        internal override void ClearMoments(bool clearBegin)
        {
            MList.Clear();
            _endMom.Time = Different.MinDate;
            if (clearBegin) _beginMom.Time = Different.MinDate;    
        }

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal override int AddMom(DateTime time, ErrMom err)  
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time <= Connect.PeriodBegin && _beginMom.Time <= time)
                _beginMom.CopyAllFrom(BufMom);
            else if (time <= Connect.PeriodEnd)
            {
                if (_endMom.Time <= time) 
                    _endMom.CopyAllFrom(BufMom);
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
        internal int MakeEnd()
        {
            if (_endMom.Time != Different.MinDate)
                _beginMom.CopyAllFrom(_endMom);
            //todo добавлять значения в клон
            return 1;
        }

        //Запись значения в клон
        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        protected override int PutClone(IMom mom) //Рекордсет срезов клона
        {
            bool isReal = DataType.LessOrEquals(DataType.Real);
            var rec = isReal ? Connect.CloneRec : Connect.CloneStrRec;
            var recCut = isReal ? Connect.CloneCutRec : Connect.CloneStrCutRec;
            int nwrite = 0;

            var d1 = Connect.RemoveMinultes(mom.Time);
            var d = Connect.RemoveMinultes(PrevMom.Time).AddMinutes(10);
            while (d <= d1)
            {
                PutCloneRec(PrevMom, recCut, true, d);
                d = d.AddMinutes(10);
                nwrite++;
            }
            PutCloneRec(mom, rec, false, mom.Time);
            PrevMom.CopyAllFrom(BufMom);
            return nwrite + 1;
        }
    }
}