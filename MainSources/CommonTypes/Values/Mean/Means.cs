using System;
using BaseLibrary;

namespace CommonTypes
{
    //Логическое значение
    public class MeanBool : MeanBoolBase
    {
        public MeanBool(bool b)
        {
            OwnBool = b;
        }
        public MeanBool() { }
        
        protected override bool OwnBool { get; set; }
    }

    //--------------------------------------------------------------------------------------------------
    //Целое значение
    public class MeanInt : MeanIntBase
    {
        public MeanInt(int i)
        {
            OwnInteger = i;
        }
        public MeanInt() { }

        protected override int OwnInteger { get; set; }
    }

    //--------------------------------------------------------------------------------------------------
    //Действительное значение
    public class MeanReal : MeanRealBase
    {
        public MeanReal(double r)
        {
            OwnReal = r;
        }
        public MeanReal() { }

        protected override double OwnReal { get; set; }
    }

    //--------------------------------------------------------------------------------------------------
    //Строковое значение
    public class MeanString : MeanStringBase
    {
        public MeanString(string s)
        {
            OwnString = s;
        }
        public MeanString() { }

        protected override string OwnString { get; set; }
    }

    //--------------------------------------------------------------------------------------------------
    //Временное значение
    public class MeanTime : MeanTimeBase
    {
        public MeanTime(DateTime d)
        {
            OwnDate = d;
        }
        public MeanTime() { }

        protected override DateTime OwnDate{ get; set; }
    }
}