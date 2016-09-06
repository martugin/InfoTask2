using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибкой

    public class MomBool : MeanBool, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }
        
        public MomBool(DateTime time, bool b) : base(b)
        {
            Time = time;
        }
        internal MomBool(){}

        public virtual IMom CloneMom()
        {
            return new MomBool(Time, Boolean);
        }

        public IMom CloneMom(ErrMom err)
        {
            return new MomErrBool(Time, Boolean, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomInt : MeanInt, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomInt(DateTime time, int i) : base(i)
        {
            Time = time;
        }
        internal MomInt() {}

        public virtual IMom CloneMom()
        {
            return new MomInt(Time, Integer);
        }

        public IMom CloneMom(ErrMom err)
        {
            return new MomErrInt(Time, Integer, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomReal : MeanReal, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomReal(DateTime time, double r) : base(r)
        {
            Time = time;
        }
        internal MomReal() {}

        public virtual IMom CloneMom()
        {
            return new MomReal(Time, Real);
        }

        public IMom CloneMom(ErrMom err)
        {
            return new MomErrReal(Time, Real, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomString : MeanString, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomString(DateTime time, string s) : base(s)
        {
            Time = time;
        }
        internal MomString() {}

        public virtual IMom CloneMom()
        {
            return new MomString(Time, String);
        }

        public IMom CloneMom(ErrMom err)
        {
            return new MomErrString(Time, String, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomTime : MeanTime, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomTime(DateTime time, DateTime d) : base(d)
        {
            Time = time;
        }
        internal MomTime() {}

        public virtual IMom CloneMom()
        {
            return new MomTime(Time, Date);
        }

        public IMom CloneMom(ErrMom err)
        {
            return new MomErrTime(Time, Date, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomWeighted : MomReal
    {
        public MomWeighted(DateTime time, double r, double w) : base(time, r)
        {
            Weight = w;
        }
        internal MomWeighted() { }

        public override DataType DataType { get { return DataType.Weighted; } }
        //Длина интервала
        public double Weight { get; internal set; }

        public override IMom CloneMom()
        {
            return new MomWeighted(Time, Real, Weight);
        }

        public override IMom CloneMom(DateTime time)
        {
            if (Error != null) return CloneMom(time, Error);
            return new MomWeighted(time, Real, Weight);
        }

        public override IMom CloneMom(DateTime time, ErrMom err)
        {
            return new MomErrWeighted(time, Real, Weight, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomValue : MeanValue, IMom
    {
        public DateTime Time { get; internal set; }
        public override DataType DataType { get { return DataType.Value; } }
        
        public MomValue(DateTime time)
        {
            Time = time;
        }
        internal MomValue() { }

        public override void ValueToRec(IRecordAdd rec, string field) { }
        public virtual IMom CloneMom()
        {
            return new MomValue(Time);
        }
        public IMom CloneMom(ErrMom err)
        {
            return new MomErrValue(Time, Error.Add(err));
        }
    }
}