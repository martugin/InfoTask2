using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал для чтения по блокам
    public abstract class SourceObject : IContextable
    {
        protected SourceObject(SourceBase source)
        {
            Source = source;
        }

        //Ссылка на источник
        protected SourceBase Source { get; private set; } 
        //Код объекта - контекст для формирования ошибок
        public string Context { get; internal set; }

        //Основной сигнал объекта
        internal protected InitialSignal ValueSignal { get; set; }
        //Список сигналов объекта
        private readonly HashSet<InitialSignal> _signals = new HashSet<InitialSignal>();
        protected HashSet<InitialSignal> Signals { get { return _signals; } }

        //Добавить к объекту сигнал, если такого еще не было
        internal InitialSignal AddSignal(InitialSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
                Signals.Add(s);
            return s;
        }
        protected virtual InitialSignal AddNewSignal(InitialSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }

        //Для объекта опредлено значение среза
        internal bool HasBegin 
        { 
            get
            {
                bool res = true;
                foreach (var sig in Signals)
                    if (sig is UniformSignal)
                        res &= ((UniformSignal) sig).HasBegin;
                return res;
            } 
        }
        
        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        internal int AddBegin()
        {
            int nwrite = 0;
            foreach (var sig in Signals)
                if (sig is UniformSignal)
                    nwrite += ((UniformSignal)sig).MakeBegin();
            return nwrite;
        }

        //Добавка мгновенных значений разного типа в указанный сигнал
        protected int AddMom(InitialSignal sig, DateTime time, bool b, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = b;
            return sig.AddMom(time, err);
        }
        protected int AddMom(InitialSignal sig, DateTime time, int i, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = i;
            return sig.AddMom(time, err);
        }
        protected int AddMom(InitialSignal sig, DateTime time, double r, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = r;
            return sig.AddMom(time, err);
        }
        protected int AddMom(InitialSignal sig, DateTime time, DateTime d, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = d;
            return sig.AddMom(time, err);
        }
        protected int AddMom(InitialSignal sig, DateTime time, string s, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = s;
            return sig.AddMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        protected int AddMom(InitialSignal sig, DateTime time, object ob, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Object = ob;
            return sig.AddMom(time, err);
        }

        //Добавка мгнорвенных значений разного типа с чтением из рекордсета
        protected int AddMomBool(InitialSignal sig, DateTime time, IRecordRead rec, string field, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = rec.GetBool(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomInt(InitialSignal sig, DateTime time, IRecordRead rec, string field, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = rec.GetInt(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomReal(InitialSignal sig, DateTime time, IRecordRead rec, string field, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = rec.GetDouble(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomString(InitialSignal sig, DateTime time, IRecordRead rec, string field, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = rec.GetString(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomTime(InitialSignal sig, DateTime time, IRecordRead rec, string field, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = rec.GetTime(field);
            return sig.AddMom(time, err);
        }

        //Создание ошибки
        protected ErrMom MakeError(int number)
        {
            return Source.MakeError(number, this);
        }

        //Добавление мгновенных значений во все сигналы объекта, используется только если источник - наследник AdoSource
        //Возвращает количество добавленных значений
        internal protected virtual int ReadMoments(IRecordRead rec) //Рекордсет, из которого читаются значения
        {
            return 0;
        }
    }
}