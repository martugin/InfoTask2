using System;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибкой

    public class MomBool : MeanBool
    {
        public override DateTime Time { get; internal set; }
        
        public MomBool(DateTime time, bool b) : base(b)
        {
            Time = time;
        }
        internal MomBool(){}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomInt : MeanInt
    {
        public override DateTime Time { get; internal set; }

        public MomInt(DateTime time, int i) : base(i)
        {
            Time = time;
        }
        internal MomInt() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomReal : MeanReal
    {
        public override DateTime Time { get; internal set; }

        public MomReal(DateTime time, double r) : base(r)
        {
            Time = time;
        }
        internal MomReal() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomString : MeanString
    {
        public override DateTime Time { get; internal set; }

        public MomString(DateTime time, string s) : base(s)
        {
            Time = time;
        }
        internal MomString() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class MomTime : MeanTime
    {
        public override DateTime Time { get; internal set; }

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
        public double Weight { get; internal set; }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomValue : MeanValue
    {
        public override DateTime Time { get; internal set; }
        
        public MomValue(DateTime time)
        {
            Time = time;
        }
        internal MomValue() { }
    }
}