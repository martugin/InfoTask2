using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал с получение срезов значений
    public class UniformSignal : ListSignal
    {
        public UniformSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
            : base(connect, code, dataType, contextOut, inf)
        {
            _beginMom = new EditMom(dataType);
            _endMom = new EditMom(dataType);    
        }

        //Тип значения
        public override SignalType Type
        {
            get { return SignalType.Uniform; }
        }

        //Значение среза на начало периода
        private readonly EditMom _beginMom;
        //Значение среза для следующего периода
        private readonly EditMom _endMom;

        //Очистка списка значений
        internal override void ClearMoments()
        {
            MomList.Clear();
            _endMom.Time = Static.MinDate;
            if (Connect.PeriodBegin != Connect.Provider.PrevPeriodEnd)
                _beginMom.Time = Static.MinDate;
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
                MomList.AddMom(BufMom); 
                return 1;
            }
            return 0;
        }

        //Для сигнала был задан срез
        internal override bool HasBegin
        {
            get { return _beginMom.Time != Static.MinDate; }
        }

        //Добавляет значение среза на начало периода в список, возвращает 1, если срез был получен, иначе 0
        internal override int MakeBegin()
        {
            if (!HasBegin) return 0;
            MomList.AddMom(_beginMom);
            return 1;
        }

        //Формирует значение на конец периода
        internal override int MakeEnd()
        {
            if (_endMom.Time != Static.MinDate)
                _beginMom.CopyAllFrom(_endMom);
            return 0;
        }
    }
}