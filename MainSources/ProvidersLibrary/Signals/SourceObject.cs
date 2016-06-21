using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал для чтения по блокам
    public class SourceObject : IContextable
    {
        public SourceObject(ISource source, string context)
        {
            Source = source;
            Context = context;
        }

        //Ссылка на источник
        protected ISource Source { get; private set; } 
        //Информация по объекту
        public string Inf { get; set; }

        //Id для добавления в файл клона
        public int IdInClone { get; set; }
        //Контекст для формирования ошибок
        public string Context { get; private set; }

        //Основной сигнал объекта
        public SourceSignal ValueSignal { get; set; }
        //Список сигналов объекта
        protected HashSet<SourceSignal> Signals = new HashSet<SourceSignal>();

        //Добавить к объекту сигнал, если такого еще не было
        public SourceSignal AddSignal(SourceSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
                Signals.Add(s);
            return s;
        }
        protected virtual SourceSignal AddNewSignal(SourceSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
        
        //Для объекта опредлено значение среза на время time
        public bool HasBegin { get; private set; }
        
        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public int AddBegin()
        {
            HasBegin = true;
            int n = 0;
            foreach (var sig in Signals)
                if (sig != null)
                    n += sig.MakeBegin();
            return n;
        }

        //Добавка мгновенных значений разного типа в указанный сигнал
        public int AddMom(SourceSignal sig, DateTime time, bool b, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = b;
            return sig.PutMom(time, err);
        }
        public int AddMom(SourceSignal sig, DateTime time, int i, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = i;
            return sig.PutMom(time, err);
        }
        public int AddMom(SourceSignal sig, DateTime time, double r, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = r;
            return sig.PutMom(time, err);
        }
        public int AddMom(SourceSignal sig, DateTime time, DateTime d, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = d;
            return sig.PutMom(time, err);
        }
        public int AddMom(SourceSignal sig, DateTime time, string s, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = s;
            return sig.PutMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        public int AddMom(SourceSignal sig, DateTime time, object ob, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Object = ob;
            return sig.PutMom(time, err);
        }

        //Создание ошибки
        public ErrMom MakeError(int number)
        {
            return Source.MakeError(number, this);
        }

        //Чтение одной строчки значений из рекордсета, возвращает количество используемых значений
        public int MakeValueFromRec(IRecordRead rec)
        {
            ValueTime = ReadTime(rec);
            ReadValue(rec);
            return AddObjectMoments();
        }
        protected virtual int AddObjectMoments()
        {
            return 0;
        }

        //Запись в клон
        #region
        //Запись характеристик объекта в таблицу CloneSignals клон
        public void WriteToClone(RecDao rec)
        {
            rec.AddNew();
            rec.Put("SignalContext", Context);
            IdInClone = rec.GetInt("Id");
            WriteObjectProperties(rec);
            rec.Update();
        }
        protected virtual void WriteObjectProperties(IRecordAdd rec) {}

        //Поля значения объекта для клона
        //Время последнего и текущего значения добавленного в клон
        protected DateTime ValueTime { get; private set; }
        protected DateTime CurValueTime { get; private set; }

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

        private DateTime RemoveMinultes(DateTime time)
        {
            int m = time.Minute;
            int k = m / Source.CloneCutFrequency;
            var d = ValueTime.AddMinutes(ValueTime.Minute).AddSeconds(ValueTime.Second).AddMilliseconds(ValueTime.Millisecond);
            return d.AddMinutes(k*m);
        }

        //Прочитать время
        protected virtual DateTime ReadTime(IRecordRead rec)
        {
            return DateTime.MinValue;
        }

        //Чтение значений из источника для клона
        protected virtual void ReadValue(IRecordRead rec) { }

        //Запись одной строчки значений из полей в клон
        protected virtual void PutValueToClone(IRecordAdd rec) { }
        #endregion
    }
}