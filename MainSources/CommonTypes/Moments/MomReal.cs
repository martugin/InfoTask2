using System;
using BaseLibrary;

namespace CommonTypes
{
    //Действительное мгновенное значение
    public class MomReal :Mom
    {
        public MomReal(ErrMom err) : base(err) { }

        //Значение
        protected double _real;

        //Тип данных
        public override DataType DataType
        {
            get { return DataType.Real; }
        }

        public override bool Boolean
        {
            get { return _real != 0; }
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

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { _real = 0; }
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

        //Сравнение значений
        public override bool ValueEquals(IMom mom)
        {
            return _real == mom.Real;
        }
        public override bool ValueLess(IMom mom)
        {
            return _real < mom.Real;
        }

        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return Create(time, Real, Error);
        }

        //Запись в рекордсет
        public override void ValueToRec(IRecordAdd rec, string field)
        {
            rec.Put(field, Real);
        }
    }
}