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
        public double Weight { get; private set; }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomWeighted(time, Real, Weight);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrWeighted(time, Real, Weight, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomValue : Mean, IMom
    {
        public DateTime Time { get; internal set; }
        public override DataType DataType { get { return DataType.Value; } }
        
        public MomValue(DateTime time)
        {
            Time = time;
        }
        internal MomValue() { }

        public override void ValueToRec(IRecordAdd rec, string field) { }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomValue(time);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrValue(time, Error.Add(err));
        }
    }
}