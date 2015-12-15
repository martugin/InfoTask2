using System;
using BaseLibrary;

namespace CommonTypes
{
    //Логическое мгновенное значение
    public class MomBoolean  : Mom
    {
        internal MomBoolean(ErrMom err) : base(err) { }

        //Значение
        private bool _boolean;

        //Тип данных
        public override DataType DataType 
        {
            get { return DataType.Boolean; }
        }

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

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _boolean = false; }
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

        //Сравнение значений
        public override bool ValueEquals(IMom mom)
        {
            return _boolean == mom.Boolean;
        }
        public override bool ValueLess(IMom mom)
        {
            return !_boolean && mom.Boolean;
        }

        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return Create(time, Boolean, Error);
        }

        //Запись в рекордсет
        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, Boolean);
        }
    }
}