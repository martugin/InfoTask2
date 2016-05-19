using System;

namespace CommonTypes
{
    //Мгновенные значения со временем и ошибками
    
    public class MomErrBool : MomBool
    {
        public override ErrMom Error { get; internal set; }
        
        public MomErrBool(DateTime time, bool b, ErrMom err) : base(time, b)
        {
            Error = err;
        }
        internal MomErrBool() {}

        public override IMom Clone()
        {
            return new MomErrBool(Time, Boolean, Error);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class MomErrInt : MomInt
    {
        public override ErrMom Error { get; internal set; }

        public MomErrInt(DateTime time, int i, ErrMom err) : base(time, i)
        {
            Error = err;
        }
        internal MomErrInt() { }

        public override IMom Clone()
        {
            return new MomErrInt(Time, Integer, Error);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class MomErrReal : MomReal
    {
        public override ErrMom Error { get; internal set; }

        public MomErrReal(DateTime time, double r, ErrMom err) : base(time, r)
        {
            Error = err;
        }
        internal MomErrReal() { }

        public override IMom Clone()
        {
            return new MomErrReal(Time, Real, Error);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class MomErrString : MomString
    {
        public override ErrMom Error { get; internal set; }

        public MomErrString(DateTime time, string s, ErrMom err)
            : base(time, s)
        {
            Error = err;
        }
        internal MomErrString() { }

        public override IMom Clone()
        {
            return new MomErrString(Time, String, Error);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class MomErrTime : MomTime
    {
        public override ErrMom Error { get; internal set; }

        public MomErrTime(DateTime time, DateTime d, ErrMom err)
            : base(time, d)
        {
            Error = err;
        }
        internal MomErrTime() { }

        public override IMom Clone()
        {
            return new MomErrTime(Time, Time, Error);
        }
    }

    //---------------------------------------------------------------------------------------------------

    public class MomErrWeighted : MomWeighted
    {
        public override ErrMom Error { get; internal set; }

        public MomErrWeighted(DateTime time, double r, double w, ErrMom err) : base(time, r, w)
        {
            Error = err;
        }
        internal MomErrWeighted() { }

        public override IMom Clone()
        {
            return new MomErrWeighted(Time, Real, Weight, Error);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public class MomErrValue : MomValue
    {
        public override ErrMom Error { get; internal set; }

        public MomErrValue(DateTime time, ErrMom err) : base(time)
        {
            Error = err;
        }
        internal MomErrValue() { }

        public override IMom Clone()
        {
            return new MomErrValue(Time, Error);
        }
    }
}