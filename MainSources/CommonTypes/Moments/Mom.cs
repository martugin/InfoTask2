using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибкой

    public class MomBool : EMeanBool, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomBool(DateTime time, bool b, ErrMom err = null) : base(b, err)
        {
            Time = time;
        }
        internal MomBool(){} 
    }

    //---------------------------------------------------------------------------------------------------

    public class MomInt : EMeanInt, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomInt(DateTime time, int i, ErrMom err = null) : base(i, err)
        {
            Time = time;
        }
        internal MomInt() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomReal : EMeanReal, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomReal(DateTime time, double r, ErrMom err = null) : base(r, err)
        {
            Time = time;
        }
        internal MomReal() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomString : EMeanString, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomString(DateTime time, string s, ErrMom err = null) : base(s, err)
        {
            Time = time;
        }
        internal MomString() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomTime : EMeanTime, IMom
    {
        //Время значения
        public DateTime Time { get; internal set; }

        public MomTime(DateTime time, DateTime d, ErrMom err = null) : base(d, err)
        {
            Time = time;
        }
        internal MomTime() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomWeighted : MomReal
    {
        public MomWeighted(DateTime time, double r, double w, ErrMom err = null) : base(time, r, err)
        {
            Weight = w;
        }
        public MomWeighted() { }

        public override DataType DataType { get { return DataType.Weighted; } }
        //Длина интервала
        public double Weight { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomValue : Mean, IMom
    {
        public DateTime Time { get; internal set; }
        public override ErrMom Error { get; internal set; }
        public override DataType DataType { get { return DataType.Value; } }
        
        public MomValue(DateTime time, ErrMom err = null)
        {
            Time = time;
            Error = err;
        }
        internal MomValue() { }

        public override void ValueToRec(IRecordAdd rec, string field) { }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomValue(time, err);
        }
    }
}