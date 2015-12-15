using System;
using BaseLibrary;

namespace CommonTypes
{
    //Целое мгновенное значение
    public class MomInteger : Mom
    {
         internal MomInteger(ErrMom err) : base(err) { }

        //Значение
        private int _integer;

        //Тип данных
        public override DataType DataType
        {
            get { return DataType.Integer; }
        }

        public override bool Boolean
        {
            get { return _integer != 0; }
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
        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _integer = 0; }
        }
        public override string String
        {
            get { return _integer.ToString(); }
            internal set { _integer = value.ToInt(); }
        }
        public override object Object
        {
            get { return _integer; }
            internal set { _integer = (int)value; }
        }

        //Сравнение значений
        public override bool ValueEquals(IMom mom)
        {
            return _integer == mom.Integer;
        }
        public override bool ValueLess(IMom mom)
        {
            return _integer < mom.Integer;
        }

        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return Create(time, Integer, Error);
        }

        //Запись в рекордсет
        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, Integer);
        }
    }
}