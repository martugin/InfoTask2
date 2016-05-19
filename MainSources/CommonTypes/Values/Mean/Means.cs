using System;
using BaseLibrary;

namespace CommonTypes
{
    //Логическое значение
    public class MeanBool : Mean
    {
        public MeanBool(bool b)
        {
            _bool = b;
        }
        public MeanBool() { }

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
            rec.Put(field, _bool);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomBool(time, _bool);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrBool(time, _bool, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Boolean = mean.Boolean;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Целое значение
    public class MeanInt : Mean
    {
        public MeanInt(int i)
        {
            _int = i;
        }
        public MeanInt() { }

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

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomInt(time, _int);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrInt(time, _int, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Integer = mean.Integer;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Действительное значение
    public class MeanReal : Mean
    {
        public MeanReal(double r)
        {
            _real = r;
        }
        public MeanReal() { }

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
            internal set { _real = (double)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _real);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomReal(time, _real);
        }

        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrReal(time, _real, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            Real = mean.Real;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Строковое значение
    public class MeanString : Mean
    {
        public MeanString(string s)
        {
            _string = s;
        }
        public MeanString() { }

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
            internal set { _string = value; }
        }
        public override object Object
        {
            get { return _string; }
            internal set { _string = (string)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _string);
        }

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomString(time, _string);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrString(time, _string, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            String = mean.String;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Временное значение
    public class MeanTime : Mean
    {
        public MeanTime(DateTime d)
        {
            _date = d;
        }
        public MeanTime() { }

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

        public override IMom Clone(DateTime time)
        {
            if (Error != null) return Clone(time, Error);
            return new MomTime(time, _date);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return new MomErrTime(time, _date, Error.Add(err));
        }

        internal override void CopyValueFrom(IMean mean)
        {
            String = mean.String;
        }
    }
}