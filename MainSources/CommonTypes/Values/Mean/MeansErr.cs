using System;

namespace CommonTypes
{
    //Значение + ошибка, но вез времени

    public class MeanErrBool : MeanBool
    {
        public override ErrMom Error { get; internal set; }
        
        public MeanErrBool(bool b, ErrMom err) : base(b)
        {
            Error = err;
        }
        internal MeanErrBool() {}
    }

    //---------------------------------------------------------------------------------------------------
    
    public class MeanErrInt : MeanInt
    {
        public override ErrMom Error { get; internal set; }

        public MeanErrInt(int i, ErrMom err) : base(i)
        {
            Error = err;
        }
        internal MeanErrInt() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrReal : MeanReal
    {
        public override ErrMom Error { get; internal set; }

        public MeanErrReal(double r, ErrMom err) : base(r)
        {
            Error = err;
        }
        internal MeanErrReal(){}
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrTime : MeanTime
    {
        public override ErrMom Error { get; internal set; }

        public MeanErrTime(DateTime t, ErrMom err) : base(t)
        {
            Error = err;
        }
        internal MeanErrTime() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrString : MeanString
    {
        public override ErrMom Error { get; internal set; }

        public MeanErrString(string s, ErrMom err) : base(s)
        {
            Error = err;
        }
        internal MeanErrString() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class MeanErrValue : MeanValue
    {
        public override ErrMom Error { get; internal set; }

        public MeanErrValue(ErrMom err)
        {
            Error = err;
        }
        internal MeanErrValue() { }
    }
}