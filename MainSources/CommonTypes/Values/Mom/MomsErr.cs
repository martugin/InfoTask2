using System;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибками
    
    public class BoolErrMom : BoolMom
    {
        public override MomErr Error { get; internal set; }
        
        public BoolErrMom(DateTime time, bool b, MomErr err) : base(time, b)
        {
            Error = err;
        }
        internal BoolErrMom() {}
    }

    //--------------------------------------------------------------------------------------------------

    public class IntErrMom : IntMom
    {
        public override MomErr Error { get; internal set; }

        public IntErrMom(DateTime time, int i, MomErr err) : base(time, i)
        {
            Error = err;
        }
        internal IntErrMom() { }
    }

    //--------------------------------------------------------------------------------------------------

    public class RealErrMom : RealMom
    {
        public override MomErr Error { get; internal set; }

        public RealErrMom(DateTime time, double r, MomErr err) : base(time, r)
        {
            Error = err;
        }
        internal RealErrMom() { }
    }

    //--------------------------------------------------------------------------------------------------

    public class StringErrMom : StringMom
    {
        public override MomErr Error { get; internal set; }

        public StringErrMom(DateTime time, string s, MomErr err)
            : base(time, s)
        {
            Error = err;
        }
        internal StringErrMom() { }
    }

    //--------------------------------------------------------------------------------------------------

    public class TimeErrMom : TimeMom
    {
        public override MomErr Error { get; internal set; }

        public TimeErrMom(DateTime time, DateTime d, MomErr err)
            : base(time, d)
        {
            Error = err;
        }
        internal TimeErrMom() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class WeightedErrMom : WeightedMom
    {
        public override MomErr Error { get; internal set; }

        public WeightedErrMom(DateTime time, double r, double w, MomErr err) : base(time, r, w)
        {
            Error = err;
        }
        internal WeightedErrMom() { }
    }

    //--------------------------------------------------------------------------------------------------

    public class ValueErrMom : ValueMom
    {
        public override MomErr Error { get; internal set; }

        public ValueErrMom(DateTime time, MomErr err) : base(time)
        {
            Error = err;
        }
        internal ValueErrMom() { }
    }
}