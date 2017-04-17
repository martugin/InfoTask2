using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал архивного источника, без работы со срезами
    //Используется для событий, сигнализации, действий оператора и т.д.
    public class CloneSignal : ListSignal
    {
        public CloneSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
            : base(connect, code, dataType, contextOut, inf) { }

        //Соединение с источником
        public ListSourceConnect SourceConnect { get { return (ListSourceConnect) Connect; } }

        //Id в таблице сигналов клона
        internal int IdInClone { get; set; }
        
        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal override int AddMom(DateTime time, MomErr err)
        {
            if (!CheckMomTime(time, err)) return 0;
            return PutClone(BufMom, false);
        }

        //Запись значения в клон, для UniformSignal переопределяется
        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        protected virtual int PutClone(IMean mom, //Рекордсет срезов клона
                                                     bool onlyCut) //Добавляет только 10-минутные срезы, но не само значение
        {
            bool isReal = DataType.LessOrEquals(DataType.Real);
            var rec = isReal ? SourceConnect.CloneRec : SourceConnect.CloneStrRec;
            PutCloneRec(mom, rec, false, mom.Time);
            return 1;
        }
        //Запись значения в рекордсет клона
        protected void PutCloneRec(IMean mom, //Значение
                                                 DaoRec rec, //Рекордсет
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