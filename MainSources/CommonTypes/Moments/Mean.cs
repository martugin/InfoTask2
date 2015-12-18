using System;
using BaseLibrary;

namespace CommonTypes
{
    //Одно значение
    public abstract class Mean : Val, IMean
    {
        public virtual bool Boolean
        {
            get { return false; }
            internal set {}
        }
        public virtual int Integer
        {
            get { return 0; }
            internal set {}
        }
        public virtual double Real
        {
            get { return 0; }
            internal set {}
        }
        public virtual DateTime Date
        {
            get { return Different.MinDate; }
            internal set {}
        }
        public virtual string String
        {
            get { return ""; }
            internal set {}
        }
        public virtual object Object
        {
            get { return 0; }
            internal set {}
        }

        public bool ValueEquals(IMean mean)
        {
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.String:
                    return String == mean.String;
                case DataType.Real:
                    return Real == mean.Real;
                case DataType.Integer:
                    return Integer == mean.Integer;
                case DataType.Boolean:
                    return Boolean == mean.Boolean;
                case DataType.Time:
                    return Date == mean.Date;
            }
            return false;
        }

        public bool ValueLess(IMean mean)
        {
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.String:
                    return String.CompareTo(mean.String) < 0;
                case DataType.Real:
                    return Real < mean.Real;
                case DataType.Integer:
                    return Integer < mean.Integer;
                case DataType.Boolean:
                    return !Boolean && mean.Boolean;
                case DataType.Time:
                    return Date < mean.Date;
            }
            return false;
        }

        public bool ValueAndErrorEquals(IMean mean)
        {
            return ValueEquals(mean) && Error == mean.Error;
        }

        public abstract void ValueToRec(IRecordAdd rec, string field);
        public abstract IMom Clone(DateTime time, ErrMom err = null);
        
        public override ICalcVal CalcValue { get { return this; } }
        public virtual ErrMom Error
        {
            get { return null; }
            internal set {}
        }
        public virtual ErrMom TotalError { get { return Error; } }
        public int Count { get { return 1; } }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanBool : Mean
    {
        public MeanBool(bool b)
        {
            _boolean = b;
        }
        public MeanBool() { }

        private bool _boolean;

        public override DataType DataType {get { return DataType.Boolean; } }
        
        public override bool Boolean
        {
            get { return _boolean; }
            internal set { _boolean = value; }
        }
        public override int Integer
        {
            get { return _boolean ? 1 : 0; }
            internal set { _boolean = value != 0; }
        }
        public override double Real
        {
            get { return _boolean ? 1 : 0; }
            internal set { _boolean = value != 0; }
        }
        public override string String
        {
            get { return _boolean ? "1" : "0"; }
            internal set { _boolean = value != "0"; }
        }
        public override object Object
        {
            get { return _boolean; }
            internal set { _boolean = (bool)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _boolean);
        }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomBool(time, _boolean, Error.Add(err));
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanInt : Mean
    {
        public MeanInt(int i)
        {
            _integer = i;
        }
        public MeanInt() { }

        private int _integer;

        public override DataType DataType { get { return DataType.Integer; } }

        public override bool Boolean
        {
            get { return _integer == 0; }
            internal set { _integer = value ? 1 : 0; }
        }
        public override int Integer
        {
            get { return _integer; }
            internal set { _integer = value; }
        }
        public override double Real
        {
            get { return _integer; }
            internal set { _integer = Convert.ToInt32(value); }
        }
        public override string String
        {
            get { return _integer.ToString(); }
            internal set { _integer = value.ToInt(); }
        }
        public override object Object
        {
            get { return _integer; }
            internal set { _integer = (int) value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _integer);
        }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomInt(time, _integer, Error.Add(err));
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanReal : Mean
    {
        public MeanReal(double r)
        {
            _real = r;
        }
        public MeanReal() { }

        private double _real;

        public override DataType DataType { get { return DataType.Real; } }

        public override bool Boolean
        {
            get { return _real == 0; }
            internal set { _real = value ? 1 : 0; }
        }
        public override int Integer
        {
            get { return Convert.ToInt32(_real); }
            internal set { _real = value; }
        }
        public override double Real
        {
            get { return _real; }
            internal set { _real = value; }
        }
        public override string String
        {
            get { return _real.ToString(); }
            internal set { _real = value.ToDouble(0); }
        }
        public override object Object
        {
            get { return _real; }
            internal set { _real = (double) value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _real);
        }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomReal(time, _real, Error.Add(err));
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanString : Mean
    {
        public MeanString(string s)
        {
            _string = s;
        }
        public MeanString() { }

        private string _string;

        public override DataType DataType { get { return DataType.String; } }

        public override bool Boolean
        {
            get { return _string == "0"; }
            internal set { _string = value ? "1" : "0"; }
        }
        public override int Integer
        {
            get { return _string.ToInt(); }
            internal set { _string = value.ToString(); }
        }
        public override double Real
        {
            get { return _string.ToDouble(); }
            internal set { _string = value.ToString(); }
        }
        public override DateTime Date
        {
            get { return _string.ToDateTime(); }
            internal set { _string = value.ToString(); }
        }
        public override string String
        {
            get { return _string; }
            internal set { _string = value; }
        }
        public override object Object
        {
            get { return _string; }
            internal set { _string = (string) value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _string);
        }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomString(time, _string, Error.Add(err));
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Логическое значение
    public class MeanTime : Mean
    {
        public MeanTime(DateTime d)
        {
            _date = d;
        }
        public MeanTime() { }

        private DateTime _date;

        public override DataType DataType { get { return DataType.Time; } }

        public override DateTime Date
        {
            get { return _date; }
            internal set { _date = value; }
        }
        public override string String
        {
            get { return _date.ToString(); }
            internal set { _date = value.ToDateTime(); }
        }
        public override object Object
        {
            get { return _date; }
            internal set { _date = (DateTime) value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _date);
        }

        public override IMom Clone(DateTime time, ErrMom err = null)
        {
            return new MomTime(time, _date, Error.Add(err));
        }
    }
}