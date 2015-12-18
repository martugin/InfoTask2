using System;

namespace CommonTypes
{
    //Значение + ошибка, но вез времени

    public class EMeanBool : MeanBool
    {
        public override ErrMom Error { get; internal set; }
        
        public EMeanBool(bool b, ErrMom err = null) : base(b)
        {
            Error = err;
        }
        internal EMeanBool() {}
    }

    //---------------------------------------------------------------------------------------------------
    
    public class EMeanInt : MeanInt
    {
        public override ErrMom Error { get; internal set; }

        public EMeanInt(int i, ErrMom err = null) : base(i)
        {
            Error = err;
        }
        internal EMeanInt() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class EMeanReal : MeanReal
    {
        public override ErrMom Error { get; internal set; }

        public EMeanReal(double r, ErrMom err = null) : base(r)
        {
            Error = err;
        }
        internal EMeanReal(){}
    }

    //---------------------------------------------------------------------------------------------------

    public class EMeanTime : MeanTime
    {
        public override ErrMom Error { get; internal set; }

        public EMeanTime(DateTime t, ErrMom err = null) : base(t)
        {
            Error = err;
        }
        internal EMeanTime() { }
    }

    //---------------------------------------------------------------------------------------------------

    public class EMeanString : MeanString
    {
        public override ErrMom Error { get; internal set; }

        public EMeanString(string s, ErrMom err = null) : base(s)
        {
            Error = err;
        }
        internal EMeanString() { }
    }
}