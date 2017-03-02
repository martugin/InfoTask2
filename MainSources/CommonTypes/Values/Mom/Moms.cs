using System;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибкой

    public class BoolMom : BoolMean
    {
        public override DateTime Time { get; internal set; }
        
        public BoolMom(DateTime time, bool b) : base(b)
        {
            Time = time;
        }
        internal BoolMom(){}
    }

    //---------------------------------------------------------------------------------------------------

    public class IntMom : IntMean
    {
        public override DateTime Time { get; internal set; }

        public IntMom(DateTime time, int i) : base(i)
        {
            Time = time;
        }
        internal IntMom() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class RealMom : RealMean
    {
        public override DateTime Time { get; internal set; }

        public RealMom(DateTime time, double r) : base(r)
        {
            Time = time;
        }
        internal RealMom() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class StringMom : StringMean
    {
        public override DateTime Time { get; internal set; }

        public StringMom(DateTime time, string s) : base(s)
        {
            Time = time;
        }
        internal StringMom() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class TimeMom : TimeMean
    {
        public override DateTime Time { get; internal set; }

        public TimeMom(DateTime time, DateTime d) : base(d)
        {
            Time = time;
        }
        internal TimeMom() {}
    }

    //---------------------------------------------------------------------------------------------------

    public class WeightedMom : RealMom
    {
        public WeightedMom(DateTime time, double r, double w) : base(time, r)
        {
            Weight = w;
        }
        internal WeightedMom() { }

        public override DataType DataType { get { return DataType.Weighted; } }
        //Длина интервала
        public double Weight { get; internal set; }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new WeightedMom(time, Real, Weight);
            return new WeightedErrMom(time, Real, Weight, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new WeightedErrMom(time, Real, Weight, Error.Add(err));
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class ValueMom : ValueMean
    {
        public override DateTime Time { get; internal set; }
        
        public ValueMom(DateTime time)
        {
            Time = time;
        }
        internal ValueMom() { }
    }
}