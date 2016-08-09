using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал, значение которого считывается из источника, без работы со срезами
    //Используется для событий, сигнализации, действий оператора и т.д.
    public class InitialSignal : SourceSignal
    {
        public InitialSignal(SourceConnect connect, string code, string codeObject, DataType dataType, string signalInf)
            : base(connect, code, codeObject, dataType, signalInf)
        {
            BufMom = new MomEdit(dataType);
        }

        //Id в таблице сигналов клона
        internal int IdInClone { get; set; }

        //Буферное значение для добавления
        internal MomEdit BufMom { get; private set; }

        //Очистка списка значений
        internal override void ClearMoments(bool clearBegin)
        {
            MList.Clear();
        }

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal virtual int AddMom(DateTime time, ErrMom err)
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time >= Connect.PeriodBegin && time <= Connect.PeriodEnd)
                return PutMom(BufMom);
            return 0;
        }

        //Запись значения в список или клон
        protected int PutMom(IMom mom)
        {
            if (IdInClone != 0) return PutClone(mom, false);
            MList.AddMom(mom);
            return 1;
        }

        //Запись значения в клон, для UniformSignal переопределяется
        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        protected virtual int PutClone(IMom mom, //Рекордсет срезов клона
                                                     bool onlyCut) //Добавляет только 10-минутные срезы, но не само значение
        {
            bool isReal = DataType.LessOrEquals(DataType.Real);
            var rec = isReal ? Connect.CloneRec : Connect.CloneStrRec;
            PutCloneRec(mom, rec, false, mom.Time);
            return 1;
        }
        //Запись значения в рекордсет клона
        protected void PutCloneRec(IMom mom, //Значение
                                                 RecDao rec, //Рекордсет
                                                 bool isCutTable, //Запись в таблицу срезов
                                                 DateTime d) //Время среза
        {
            rec.AddNew();
            rec.Put("SignalId", IdInClone);
            if (isCutTable) rec.Put("CutTime", d);
            rec.Put("Time", mom.Time);
            if (mom.Error != null)
                rec.Put("ErrNum", mom.Error.Number);
            if (DataType.LessOrEquals(DataType.Real))
                rec.Put("RealValue", mom.Real);
            else if (DataType == DataType.String)
                rec.Put("StrValue", mom.String);
            else if (DataType == DataType.Time)
                rec.Put("TimeValue", mom.Date);
            rec.Update();
        }
    }
}