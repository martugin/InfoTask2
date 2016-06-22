using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    public class OleDbSourceObject : SourceObject
    {
        //Чтение одной строчки значений из рекордсета, возвращает количество используемых значений
        public OleDbSourceObject(ISource source, string context) : 
            base(source, context) { }

        public int MakeValueFromRec(IRecordRead rec)
        {
            ValueTime = ReadTime(rec);
            ReadValue(rec);
            return AddObjectMoments();
        }

        //Чтение одной строчки значений из рекордсета, и запись ее в клон
        public int ReadValueToClone(IRecordRead rec, //Исходный рекордсет
                                                   IRecordAdd recClone, //Рекордсет клона
                                                   IRecordAdd recCut) //Рекордсет срезов клона
        {
            int nwrite = 0;
            CurValueTime = ReadTime(rec);
            var d1 = RemoveMinultes(CurValueTime);
            var d = RemoveMinultes(ValueTime).AddMinutes(Source.CloneCutFrequency);
            while (d <= d1)
            {
                recCut.AddNew();
                recCut.Put("ValueTime", d);
                PutValueToClone(recCut);
                recCut.Update();
                d = d.AddMinutes(Source.CloneCutFrequency);
                nwrite++;
            }
            ValueTime = CurValueTime;
            ReadValue(rec);
            recClone.AddNew();
            recCut.Put("ValueTime", ValueTime);
            PutValueToClone(recClone);
            recClone.Update();
            return nwrite + 1;
        }

        //Прочитать время
        protected virtual DateTime ReadTime(IRecordRead rec)
        {
            return DateTime.MinValue;
        }

        //Чтение значений из источника для клона
        protected virtual void ReadValue(IRecordRead rec) { }
    }
}