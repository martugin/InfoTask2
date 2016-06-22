using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
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
        
        //Для объекта опредлено значение среза
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

        protected virtual int AddObjectMoments()
        {
            return 0;
        }

        //Запись в клон
        #region
        //Запись характеристик объекта в таблицу CloneSignals клон
        public void WritePropsToClone(RecDao rec)
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
        protected DateTime ValueTime { get; set; }
        protected DateTime CurValueTime { get; set; }

        protected DateTime RemoveMinultes(DateTime time)
        {
            int m = time.Minute;
            int k = m / Source.CloneCutFrequency;
            var d = ValueTime.AddMinutes(ValueTime.Minute).AddSeconds(ValueTime.Second).AddMilliseconds(ValueTime.Millisecond);
            return d.AddMinutes(k*m);
        }
        
        //Запись одной строчки значений из полей в клон
        protected virtual void PutValueToClone(IRecordAdd rec) { }
        #endregion
    }
}