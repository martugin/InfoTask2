using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал для чтения по блокам
    public abstract class SourceObject : IContextable
    {
        protected SourceObject(Source source)
        {
            Source = source;
        }

        //Ссылка на источник
        protected Source Source { get; private set; } 
        //Код объекта для формирования ошибок
        public string CodeObject { get; internal set; }

        //Основной сигнал объекта
        public SourceInitSignal ValueSignal { get; set; }
        //Список сигналов объекта
        protected HashSet<SourceInitSignal> Signals = new HashSet<SourceInitSignal>();

        //Добавить к объекту сигнал, если такого еще не было
        public SourceInitSignal AddSignal(SourceInitSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
            {
                Signals.Add(s);
                s.SourceObject = this;
            }
            return s;
        }
        protected virtual SourceInitSignal AddNewSignal(SourceInitSignal sig)
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
        public int AddMom(SourceInitSignal sig, DateTime time, bool b, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = b;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourceInitSignal sig, DateTime time, int i, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = i;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourceInitSignal sig, DateTime time, double r, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = r;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourceInitSignal sig, DateTime time, DateTime d, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = d;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourceInitSignal sig, DateTime time, string s, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = s;
            return sig.AddMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        public int AddMom(SourceInitSignal sig, DateTime time, object ob, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Object = ob;
            return sig.AddMom(time, err);
        }

        //Создание ошибки
        public ErrMom MakeError(int number)
        {
            return Source.MakeError(number, this);
        }

        //Добавление мгновенных значений во все сигналы объекта, только если источник - наследник AdoSource
        public virtual int ReadMoments(IRecordRead rec) //Рекордсет, из которого читаются значения
        {
            return 0;
        }
    }
}