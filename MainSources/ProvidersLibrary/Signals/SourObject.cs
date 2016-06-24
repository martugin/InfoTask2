using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал для чтения по блокам
    public abstract class SourObject : IContextable
    {
        protected SourObject(SourConn conn)
        {
            SourceConn = conn;
        }

        //Ссылка на источник
        protected SourConn SourceConn { get; private set; } 
        //Код объекта для формирования ошибок
        public string CodeObject { get; internal set; }

        //Основной сигнал объекта
        public SourInitSignal ValueSignal { get; set; }
        //Список сигналов объекта
        protected HashSet<SourInitSignal> Signals = new HashSet<SourInitSignal>();

        //Добавить к объекту сигнал, если такого еще не было
        public SourInitSignal AddSignal(SourInitSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
            {
                Signals.Add(s);
                s.SourObject = this;
            }
            return s;
        }
        protected virtual SourInitSignal AddNewSignal(SourInitSignal sig)
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
        public int AddMom(SourInitSignal sig, DateTime time, bool b, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = b;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourInitSignal sig, DateTime time, int i, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = i;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourInitSignal sig, DateTime time, double r, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = r;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourInitSignal sig, DateTime time, DateTime d, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = d;
            return sig.AddMom(time, err);
        }
        public int AddMom(SourInitSignal sig, DateTime time, string s, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = s;
            return sig.AddMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        public int AddMom(SourInitSignal sig, DateTime time, object ob, ErrMom err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Object = ob;
            return sig.AddMom(time, err);
        }

        //Создание ошибки
        public ErrMom MakeError(int number)
        {
            return SourceConn.MakeError(number, this);
        }

        //Добавление мгновенных значений во все сигналы объекта, только если источник - наследник AdoSour
        public virtual int ReadMoments(IRecordRead rec) //Рекордсет, из которого читаются значения
        {
            return 0;
        }
    }
}