using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;

namespace CommonTypes
{
    //Один сигнал для чтения по блокам
    public class SourceObject : IContextable
    {
        public SourceObject(SourceBase source)
        {
            Source = source;
        }

        //Ссылка на источник
        protected SourceBase Source { get; private set; } 
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
        protected int AddMom(SourceSignal sig, DateTime time, bool b, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, b, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, int i, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, i, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, double r, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, r, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, DateTime d, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, d, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, string s, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, s, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        protected int AddMom(SourceSignal sig, DateTime time, object ob, ErrMom err = null)
        {
            if (sig == null) return 0;
            return sig.AddMom(time, ob, err);
        }

        //Создание ошибки 
        protected ErrMom MakeError(int number)
        {
            return Source.MakeError(number, this);
        }
    }
}