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
            set { _bool = value; }
        }
        public override int Integer
        {
            get { return _bool ? 1 : 0; }
            set { _bool = value != 0; }
        }
        public override double Real
        {
            get { return _bool ? 1.0 : 0.0; }
            set { _bool = value != 0.0; }
        }
        public override string String
        {
            get { return _bool ? "1" : "0"; }
            set { _bool = value != "0"; }
        }
        public override object Object
        {
            get { return _bool; }
            set { _bool = (bool)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            try { rec.Put(field, _bool ? 1 : 0);}
            catch { rec.Put(field, _bool); }
        }

        public override void ValueFromRec(IRecordRead rec, string field)
        {
            rec.GetBool(field);
        }

        public override IReadMean ToMean()
        {
            return new BoolMean(_bool);
        }
        
        public override IReadMean ToMom(DateTime time)
        {
            return new BoolMom(time, _bool, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new BoolMom(time, _bool, Error.Add(err));
        }

        internal override void CopyValueFrom(IReadMean mean)
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
            set { _int = value ? 1 : 0; }
        }
        public override int Integer
        {
            get { return _int; }
            set { _int = value; }
        }
        public override double Real
        {
            get { return _int; }
            set { _int = Convert.ToInt32(value); }
        }
        public override string String
        {
            get { return _int.ToString(); }
            set { _int = value.ToInt(); }
        }
        public override object Object
        {
            get { return _int; }
            set { _int = (int)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _int);
        }

        public override void ValueFromRec(IRecordRead rec, string field)
        {
            rec.GetInt(field);
        }

        public override IReadMean ToMean()
        {
            return new IntMean(_int);
        }
        
        public override IReadMean ToMom(DateTime time)
        {
            return new IntMom(time, _int, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new IntMom(time, _int, Error.Add(err));
        }

        internal override void CopyValueFrom(IReadMean mean)
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
            set { _real = value ? 1.0 : 0.0; }
        }
        public override int Integer
        {
            get { try { return Convert.ToInt32(_real); }
                    catch { return 0; } }
            set { _real = value; }
        }
        public override double Real
        {
            get { return _real; }
            set { _real = value; }
        }
        public override string String
        {
            get { return _real.ToString(); }
            set { _real = value.ToDouble(0); }
        }
        public override object Object
        {
            get { return _real; }
            set { _real = (double)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _real);
        }

        public override void ValueFromRec(IRecordRead rec, string field)
        {
            rec.GetDouble(field);
        }

        public override IReadMean ToMean()
        {
            return new RealMean(_real);
        }

        public override IReadMean ToMom(DateTime time)
        {
            return new RealMom(time, _real, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new RealMom(time, _real, Error.Add(err));
        }

        internal override void CopyValueFrom(IReadMean mean)
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
            set { _string = value ? "1" : "0"; }
        }
        public override int Integer
        {
            get { return _string.ToInt(); }
            set { _string = value.ToString(); }
        }
        public override double Real
        {
            get { return _string.ToDouble(); }
            set { _string = value.ToString(); }
        }
        public override DateTime Date
        {
            get { return _string.ToDateTime(); }
            set { _string = value.ToString(); }
        }
        public override string String
        {
            get { return _string; }
            set { _string = value ?? ""; }
        }
        public override object Object
        {
            get { return _string; }
            set { _string = (string)value ?? ""; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _string);
        }

        public override void ValueFromRec(IRecordRead rec, string field)
        {
            rec.GetString(field);
        }

        public override IReadMean ToMean()
        {
            return new StringMean(_string);
        }

        public override IReadMean ToMom(DateTime time)
        {
            return new StringMom(time, _string, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new StringMom(time, _string, Error.Add(err));
        }

        internal override void CopyValueFrom(IReadMean mean)
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
            set { _date = value; }
        }
        public override string String
        {
            get { return _date.ToString(); }
            set { _date = value.ToDateTime(); }
        }
        public override object Object
        {
            get { return _date; }
            set { _date = (DateTime)value; }
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, _date);
        }

        public override void ValueFromRec(IRecordRead rec, string field)
        {
            rec.GetTime(field);
        }

        public override IReadMean ToMean()
        {
            return new TimeMean(_date);
        }

        public override IReadMean ToMom(DateTime time)
        {
            return new TimeMom(time, _date, Error);
        }

        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new TimeMom(time, _date, Error.Add(err));
        }

        internal override void CopyValueFrom(IReadMean mean)
        {
            String = mean.String;
        }

        internal override void MakeDefaultValue()
        {
            Date = Static.MinDate;
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
        public override void ValueFromRec(IRecordRead rec, string field) { }
        public override IReadMean ToMean()
        {
            return new ValueMean();
        }
        public override IReadMean ToMom(DateTime time)
        {
            return new ValueMom(time);
        }
        public override IReadMean ToMom(DateTime time, MomErr err)
        {
            return new ValueMom(time, Error.Add(err));
        }
        internal override void CopyValueFrom(IReadMean mean) { }
        internal override void MakeDefaultValue() { }
    }
}