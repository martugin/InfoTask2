using System;
using BaseLibrary;

namespace CommonTypes
{
    //Базовые классы для одиночных значений и списков значений по типам данных

    internal class MeanBoolBase : Mean
    {
        //Логическое значение
        protected abstract bool OwnBool { get; set; }

        //Тип данных
        public override DataType DataType { get { return DataType.Boolean; } }
        
        public override bool Boolean
        {
            get { return OwnBool; }
            internal set { OwnBool = value; }
        }
        public override int Integer
        {
            get { return OwnBool ? 1 : 0; }
            internal set { OwnBool = value != 0; }
        }
        public override double Real
        {
            get { return OwnBool ? 1.0 : 0.0; }
            internal set { OwnBool = value != 0.0; }
        }
        public override string String
        {
            get { return OwnBool ? "1" : "0"; }
            internal set { OwnBool = value != "0"; }
        }
        public override object Object
        {
            get { return OwnBool; }
            internal set { OwnBool = (bool)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, OwnBool);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomBool(time, OwnBool);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrBool(time, OwnBool, Error.Add(err));
        }

        internal override void GetValueFromMean(IMean mean)
        {
            Boolean = mean.Boolean;
        }
    }

    //------------------------------------------------------------------------------------------------

    public abstract class MeanIntBase : Mean
    {
        //Целое значение
        protected abstract int OwnInteger { get; set; }

        public override DataType DataType { get { return DataType.Integer; } }

        public override bool Boolean
        {
            get { return OwnInteger == 0; }
            internal set { OwnInteger = value ? 1 : 0; }
        }
        public override int Integer
        {
            get { return OwnInteger; }
            internal set { OwnInteger = value; }
        }
        public override double Real
        {
            get { return OwnInteger; }
            internal set { OwnInteger = Convert.ToInt32(value); }
        }
        public override string String
        {
            get { return OwnInteger.ToString(); }
            internal set { OwnInteger = value.ToInt(); }
        }
        public override object Object
        {
            get { return OwnInteger; }
            internal set { OwnInteger = (int)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, OwnInteger);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomInt(time, OwnInteger);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrInt(time, OwnInteger, Error.Add(err));
        }

        internal override void GetValueFromMean(IMean mean)
        {
            Integer = mean.Integer;
        }
    }

    //------------------------------------------------------------------------------------------------

    public abstract class MeanRealBase : Mean
    {
        //Значение
        protected abstract double OwnReal { get; set; }

        public override DataType DataType { get { return DataType.Real; } }

        public override bool Boolean
        {
            get { return OwnReal == 0.0; }
            internal set { OwnReal = value ? 1.0 : 0.0; }
        }
        public override int Integer
        {
            get { return Convert.ToInt32(OwnReal); }
            internal set { OwnReal = value; }
        }
        public override double Real
        {
            get { return OwnReal; }
            internal set { OwnReal = value; }
        }
        public override string String
        {
            get { return OwnReal.ToString(); }
            internal set { OwnReal = value.ToDouble(0); }
        }
        public override object Object
        {
            get { return OwnReal; }
            internal set { OwnReal = (double)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, OwnReal);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomReal(time, OwnReal);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrReal(time, OwnReal, Error.Add(err));
        }

        internal override void GetValueFromMean(IMean mean)
        {
            Real = mean.Real;
        }
    }

    //------------------------------------------------------------------------------------------------

    public abstract class MeanStringBase : Mean
    {
        //Строковое значение
        protected abstract string OwnString { get; set; }

        public override DataType DataType { get { return DataType.String; } }

        public override bool Boolean
        {
            get { return OwnString == "0"; }
            internal set { OwnString = value ? "1" : "0"; }
        }
        public override int Integer
        {
            get { return OwnString.ToInt(); }
            internal set { OwnString = value.ToString(); }
        }
        public override double Real
        {
            get { return OwnString.ToDouble(); }
            internal set { OwnString = value.ToString(); }
        }
        public override DateTime Date
        {
            get { return OwnString.ToDateTime(); }
            internal set { OwnString = value.ToString(); }
        }
        public override string String
        {
            get { return OwnString; }
            internal set { OwnString = value; }
        }
        public override object Object
        {
            get { return OwnString; }
            internal set { OwnString = (string)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, OwnString);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomString(time, OwnString);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrString(time, OwnString, Error.Add(err));
        }

        internal override void GetValueFromMean(IMean mean)
        {
            Date = mean.Date;
        }
    }

    //------------------------------------------------------------------------------------------------

    public abstract class MeanTimeBase : Mean
    {
        //Строковое значение
        protected abstract DateTime OwnDate { get; set; }

        public override DataType DataType { get { return DataType.Time; } }

        public override DateTime Date
        {
            get { return OwnDate; }
            internal set { OwnDate = value; }
        }
        public override string String
        {
            get { return OwnDate.ToString(); }
            internal set { OwnDate = value.ToDateTime(); }
        }
        public override object Object
        {
            get { return OwnDate; }
            internal set { OwnDate = (DateTime)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, OwnDate);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomTime(time, OwnDate);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrTime(time, OwnDate, Error.Add(err));
        }

        internal override void GetValueFromMean(IMean mean)
        {
            String = mean.String;
        }
    }
}