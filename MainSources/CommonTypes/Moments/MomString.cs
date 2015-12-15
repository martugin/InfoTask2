using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomString : Mom
    {
        public MomString(ErrMom err) : base(err) { }

        //Значение
        private string _string;

        //Тип данных
        public override DataType DataType
        {
            get { return DataType.String; }
        }

        public override bool Boolean
        {
            get { return _string != "0"; }
            internal set { _string = value ? "1" : "0"; }
        }

        public override int Integer
        {
            get { int i; return Int32.TryParse(_string, out i) ? i : 0; }
            internal set { _string = value.ToString(); }
        }

        public override double Real
        {
            get
            {
                double d = _string.ToDouble();
                if (Double.IsNaN(d)) d = 0;
                return d;
            }
            internal set { _string = value.ToString(); }
        }

        public override DateTime Date
        {
            get
            {
                DateTime t;
                return DateTime.TryParse(_string, out t) ? t : Different.MinDate;
            }
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

        //Сравнение значений
        public override bool ValueEquals(IMom mom)
        {
            return _string == mom.String;
        }
        public override bool ValueLess(IMom mom)
        {
            if (_string.IsEmpty() || mom.String.IsEmpty()) return false;
            return _string.CompareTo(mom.String) < 0;
        }

        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return Create(time, String, Error);
        }

        //Запись в рекордсет
        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, String);
        }
    }
}