﻿using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал архивного источника, с работой со срезами
    public class UniformSignal : CloneSignal
    {
        public UniformSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
            : base(connect, code, dataType, contextOut, inf)
        {
            _prevMom = new EditMom(dataType);
            _beginMom = new EditMom(dataType);
            _endMom = new EditMom(dataType);    
        }

        //Значение среза на начало периода
        private readonly EditMom _beginMom;
        //Значение среза для следующего периода
        private readonly EditMom _endMom;
        //Предыдущее значение, добавленное в клон
        private readonly EditMom _prevMom;

        //Очистка списка значений
        internal override void ClearMoments(bool clearBegin)
        {
            MomList.Clear();
            _endMom.Time = Static.MinDate;
            if (clearBegin) _beginMom.Time = Static.MinDate;    
        }

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal override int AddMom(DateTime time, MomErr err)  
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time <= Connect.PeriodBegin)
            {
                if (_beginMom.Time <= time)
                    _beginMom.CopyAllFrom(BufMom);
            }
            else if (time <= Connect.PeriodEnd)
            {
                if (_endMom.Time <= time)
                    _endMom.CopyAllFrom(BufMom);
                return PutMom(BufMom);
            }
            return 0;
        }

        //Для сигнала был задан срез
        internal bool HasBegin
        {
            get { return _beginMom.Time != Static.MinDate; }
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        internal int MakeBegin()
        {
            return HasBegin ? PutMom(_beginMom) : 0;
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        internal int MakeEnd()
        {
            if (_endMom.Time != Static.MinDate)
                _beginMom.CopyAllFrom(_endMom);
            BufMom.CopyValueFrom(_endMom);
            BufMom.Time = Connect.PeriodEnd;
            if (IdInClone == 0) return 0;
            return PutClone(BufMom, true);
        }

        //Запись значения в клон
        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        protected override int PutClone(IMean mom, //Рекордсет срезов клона
                                                       bool onlyCut) //Добавляет только 10-минутные срезы, но не само значение
        {
            bool isReal = DataType.LessOrEquals(DataType.Real);
            var rec = isReal ? SourceConnect.CloneRec : SourceConnect.CloneStrRec;
            var recCut = isReal ? SourceConnect.CloneCutRec : SourceConnect.CloneStrCutRec;
            int nwrite = 0;
            if (_prevMom.Time >= Connect.PeriodBegin)
            {
                var d1 = SourceConnect.RemoveMinultes(mom.Time);
                var d = SourceConnect.RemoveMinultes(_prevMom.Time).AddMinutes(10);
                while (d <= d1)
                {
                    if (d != mom.Time)
                    {
                        PutCloneRec(_prevMom, recCut, true, d);
                        nwrite++;
                    }
                    d = d.AddMinutes(10);
                }    
            }
            if (!onlyCut)
            {
                PutCloneRec(mom, rec, false, mom.Time);
                nwrite++;
            }
            _prevMom.CopyAllFrom(BufMom);
            return nwrite;
        }
    }
}