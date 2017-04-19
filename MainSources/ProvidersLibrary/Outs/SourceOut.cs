using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Выход источника
    public abstract class SourceOut : ProviderOut
    {
        protected SourceOut(Source provider) 
            : base(provider) { }

        //Ссылка на источник
        protected Source Source
        {
            get { return (Source)Provider; }
        }

        //Добавка мгновенных значений разного типа в указанный сигнал
        protected int AddMom(SourceSignal sig, DateTime time, bool b, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = b;
            return sig.AddMom(time, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, int i, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = i;
            return sig.AddMom(time, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, double r, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = r;
            return sig.AddMom(time, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, DateTime d, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = d;
            return sig.AddMom(time, err);
        }
        protected int AddMom(SourceSignal sig, DateTime time, string s, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = s;
            return sig.AddMom(time, err);
        }
        //Добавка мгновенных значений, значение берется из типа object
        protected int AddMom(SourceSignal sig, DateTime time, object ob, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Object = ob;
            return sig.AddMom(time, err);
        }

        //Добавка мгнорвенных значений разного типа с чтением из рекордсета
        protected int AddMomBool(SourceSignal sig, DateTime time, IRecordRead rec, string field, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Boolean = rec.GetBool(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomInt(SourceSignal sig, DateTime time, IRecordRead rec, string field, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Integer = rec.GetInt(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomReal(SourceSignal sig, DateTime time, IRecordRead rec, string field, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Real = rec.GetDouble(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomString(SourceSignal sig, DateTime time, IRecordRead rec, string field, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.String = rec.GetString(field);
            return sig.AddMom(time, err);
        }
        protected int AddMomTime(SourceSignal sig, DateTime time, IRecordRead rec, string field, MomErr err = null)
        {
            if (sig == null) return 0;
            sig.BufMom.Date = rec.GetTime(field);
            return sig.AddMom(time, err);
        }

        //Создание ошибки
        protected MomErr MakeError(int number)
        {
            return Source.MakeError(number, this);
        }
    }
}