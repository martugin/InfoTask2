using System;
using BaseLibrary;

namespace CommonTypes
{
    //Логическое значение
    public class BoolMean : Mean
    {
        public BoolMean(bool b)
        {
            _bool = b;
        }
        internal BoolMean() { }

        //Значение
        private bool _bool;
        //Тип данных
        public override DataType DataType { get { return DataType.Boolean; } }

        //Преобразование типа значения
        public override bool Boolean
        {
            get { return _bool; }
            internal set { _bool = value; }
        }
        public override int Integer
        {
            get { return _bool ? 1 : 0; }
            internal set { _bool = value != 0; }
        }
        public override double Real
        {
            get { return _bool ? 1.0 : 0.0; }
            internal set { _bool = value != 0.0; }
        }
        public override string String
        {
            get { return _bool ? "1" : "0"; }
            internal set { _bool = value != "0"; }
        }
        public override object Object
        {
            get { return _bool; }
            internal set { _bool = (bool)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            try { rec.Put(field, _bool ? 1 : 0);}
            catch { rec.Put(field, _bool); }
        }

        public override IMean ToMean()
        {
            if (Error == null) return new BoolMean(_bool);
            return new MeanErrBool(_bool, Error);
        }

        public override IMean ToMean(MomErr err)
        {
            return new MeanErrBool(_bool, Error.Add(err));
        }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new BoolMom(time, _bool);
            return new BoolErrMom(time, _bool, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new BoolErrMom(time, _bool, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Boolean = mean.Boolean;
        }

        internal override void MakeDefaultValue()
        {
            Boolean = false;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Целое значение
    public class IntMean : Mean
    {
        public IntMean(int i)
        {
            _int = i;
        }
        public IntMean() { }

        //Значение
        private int _int;
        //Тип данных
        public override DataType DataType { get { return DataType.Integer; } }

        //Преобразование типа значения
        public override bool Boolean
        {
            get { return _int != 0; }
            internal set { _int = value ? 1 : 0; }
        }
        public override int Integer
        {
            get { return _int; }
            internal set { _int = value; }
        }
        public override double Real
        {
            get { return _int; }
            internal set { _int = Convert.ToInt32(value); }
        }
        public override string String
        {
            get { return _int.ToString(); }
            internal set { _int = value.ToInt(); }
        }
        public override object Object
        {
            get { return _int; }
            internal set { _int = (int)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _int);
        }

        public override IMean ToMean()
        {
            if (Error == null) return new IntMean(_int);
            return new MeanErrInt(_int, Error);
        }

        public override IMean ToMean(MomErr err)
        {
            return new MeanErrInt(_int, Error.Add(err));
        }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new IntMom(time, _int);
            return new IntErrMom(time, _int, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new IntErrMom(time, _int, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Integer = mean.Integer;
        }

        internal override void MakeDefaultValue()
        {
            Integer = 0;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Действительное значение
    public class RealMean : Mean
    {
        public RealMean(double r)
        {
            _real = r;
        }
        public RealMean() { }

        //Значение
        private double _real;
        //Тип данных
        public override DataType DataType { get { return DataType.Real; } }

        //Преобразование типа значения
        public override bool Boolean
        {
            get { return _real != 0.0; }
            internal set { _real = value ? 1.0 : 0.0; }
        }
        public override int Integer
        {
            get { try { return Convert.ToInt32(_real); }
                    catch { return 0; } }
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
            internal set { _real = (double)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _real);
        }

        public override IMean ToMean()
        {
            if (Error == null) return new RealMean(_real);
            return new MeanErrReal(_real, Error);
        }

        public override IMean ToMean(MomErr err)
        {
            return new MeanErrReal(_real, Error.Add(err));
        }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new RealMom(time, _real);
            return new RealErrMom(time, _real, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new RealErrMom(time, _real, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Real = mean.Real;
        }

        internal override void MakeDefaultValue()
        {
            Real = 0;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Строковое значение
    public class StringMean : Mean
    {
        public StringMean(string s)
        {
            _string = s ?? "";
        }
        public StringMean() { }

        //Значение
        private string _string;
        //Тип данных
        public override DataType DataType { get { return DataType.String; } }

        //Преобразование типа значения
        public override bool Boolean
        {
            get { return _string != "0"; }
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
            internal set { _string = value ?? ""; }
        }
        public override object Object
        {
            get { return _string; }
            internal set { _string = (string)value ?? ""; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _string);
        }

        public override IMean ToMean()
        {
            if (Error == null) return new StringMean(_string);
            return new MeanErrString(_string, Error);
        }

        public override IMean ToMean(MomErr err)
        {
            return new MeanErrString(_string, Error.Add(err));
        }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new StringMom(time, _string);
            return new StringErrMom(time, _string, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new StringErrMom(time, _string, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            String = mean.String;
        }

        internal override void MakeDefaultValue()
        {
            String = "";
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Временное значение
    public class TimeMean : Mean
    {
        public TimeMean(DateTime d)
        {
            _date = d;
        }
        public TimeMean() { }

        //Значение
        private DateTime _date;
        //Тип данных
        public override DataType DataType { get { return DataType.Time; } }

        //Преобразование типа значения
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
            internal set { _date = (DateTime)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _date);
        }

        public override IMean ToMean()
        {
            if (Error == null) return new TimeMean(_date);
            return new MeanErrTime(_date, Error);
        }

        public override IMean ToMean(MomErr err)
        {
            return new MeanErrTime(_date, Error.Add(err));
        }

        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new TimeMom(time, _date);
            return new TimeErrMom(time, _date, Error);
        }

        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new TimeErrMom(time, _date, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            String = mean.String;
        }

        internal override void MakeDefaultValue()
        {
            Date = Different.MinDate;
        }
    }

    //-------------------------------------------------------------------------------------
    public class ValueMean : Mean
    {
        //Тип данных
        public override DataType DataType { get { return DataType.Value; } }

        //Преобразование типа значения
        public override bool Boolean { get { return false; } }
        public override int Integer { get { return 0; } }
        public override double Real { get { return 0.0; } }
        public override string String { get { return "0"; } }
        public override object Object { get { return 0; } }

        public override void ValueToRec(IRecordAdd rec, string field) { }
        public override IMean ToMean()
        {
            if (Error == null) return new ValueMean();
            return new MeanErrValue(Error);
        }
        public override IMean ToMean(MomErr err)
        {
            return new MeanErrValue(Error.Add(err));
        }
        public override IMean ToMom(DateTime time)
        {
            if (Error == null) return new ValueMom(time);
            return new ValueErrMom(time, Error);
        }
        public override IMean ToMom(DateTime time, MomErr err)
        {
            return new ValueErrMom(time, Error.Add(err));
        }
        internal override void CopyValueFrom(IMean mean) { }
        internal override void MakeDefaultValue() { }
    }
}