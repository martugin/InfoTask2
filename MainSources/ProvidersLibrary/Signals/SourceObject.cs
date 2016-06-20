using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал для чтения по блокам
    public class SourceObject : IContextable
    {
        public SourceObject(ISource source)
        {
            Source = source;
        }

        //Ссылка на источник
        protected ISource Source { get; private set; } 
        //Информация по объекту
        public string Inf { get; set; }

        //Контекст для формирования ошибок
        public virtual string Context 
        { 
            get 
            { 
                if (ValueSignal != null) 
                    return ValueSignal.Context;
                if (Signals.Count > 0)
                    return Signals.First().Context;
                return "";
            } 
        }

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

        //Чтение одной строчки значений из рекордсета, возвращает количество используемых значений
        public virtual int ReadValueFromRec(IRecordRead rec)
        {
            return 0;
        }

        //Для объекта опредлено значение среза на время time
        public bool HasBegin
        {
            get
            {
                bool e = true;
                foreach (var sig in Signals)
                    if (sig != null)
                        e &= sig.HasBegin;
                return e;    
            }
        }
        
        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public int AddBegin()
        {
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
    }
}