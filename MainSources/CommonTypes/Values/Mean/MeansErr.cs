using System;

namespace CommonTypes
{
    //Значение + ошибка, но вез времени

    public class MeanErrBool : BoolMean
    {
        public override MomErr Error { get; internal set; }
        
        public MeanErrBool(bool b, MomErr err) : base(b)
        {
            Error = err;
        }
        internal MeanErrBool() {}
    }

    //---------------------------------------------------------------------------------------------------
    
    public class MeanErrInt : IntMean
    {
        public override MomErr Error { get; internal set; }

        public MeanErrInt(int i, MomErr err) : base(i)
        {
            Error = err;
        }
        internal MeanErrInt() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrReal : RealMean
    {
        public override MomErr Error { get; internal set; }

        public MeanErrReal(double r, MomErr err) : base(r)
        {
            Error = err;
        }
        internal MeanErrReal(){}
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrTime : TimeMean
    {
        public override MomErr Error { get; internal set; }

        public MeanErrTime(DateTime t, MomErr err) : base(t)
        {
            Error = err;
        }
        internal MeanErrTime() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrString : StringMean
    {
        public override MomErr Error { get; internal set; }

        public MeanErrString(string s, MomErr err) : base(s)
        {
            Error = err;
        }
        internal MeanErrString() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrValue : ValueMean
    {
        public override MomErr Error { get; internal set; }

        public MeanErrValue(MomErr err)
        {
            Error = err;
        }
        internal MeanErrValue() { }
    }
}