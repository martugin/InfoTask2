using System;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибкой

    public class BoolMom : BoolMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }
        
        public BoolMom(DateTime time, bool b, MomErr err = null) : base(b)
        {
            Time = time;
            Error = err;
        }
        internal BoolMom(){}

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class IntMom : IntMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }

        public IntMom(DateTime time, int i, MomErr err = null)
            : base(i)
        {
            Time = time;
            Error = err;
        }
        internal IntMom() {}

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class RealMom : RealMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }

        public RealMom(DateTime time, double r, MomErr err = null)
            : base(r)
        {
            Time = time;
            Error = err;
        }
        internal RealMom() {}

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class StringMom : StringMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }

        public StringMom(DateTime time, string s, MomErr err = null)
            : base(s)
        {
            Time = time;
            Error = err;
        }
        internal StringMom() {}

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class TimeMom : TimeMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }

        public TimeMom(DateTime time, DateTime d, MomErr err = null)
            : base(d)
        {
            Time = time;
            Error = err;
        }
        internal TimeMom() {}

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class WeightedMom : RealMom
    {
        public WeightedMom(DateTime time, double r, double w, MomErr err = null)
            : base(time, r, err)
        {
            Weight = w;
        }
        internal WeightedMom() { }

        public override DataType DataType { get { return DataType.Weighted; } }
        //Длина интервала
        public double Weight { get; internal set; }

        public override IReadMean ToMom(DateTime time)
        {
            return new WeightedMom(time, Real, Weight, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new WeightedMom(time, Real, Weight, Error.Add(err));
        }

        //Клонировать
        public override object Clone() { return ToMom(); }
    }

    //---------------------------------------------------------------------------------------------------

    public class ValueMom : ValueMean
    {
        public sealed override DateTime Time { get; set; }
        public sealed override MomErr Error { get; set; }
        
        public ValueMom(DateTime time, MomErr err = null)
        {
            Time = time;
            Error = err;
        }
        internal ValueMom() { }

        //Клонировать
        public override object Clone() { return ToMom(); }
    }
}