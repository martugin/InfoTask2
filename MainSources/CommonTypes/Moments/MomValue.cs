using System;
using BaseLibrary;

namespace CommonTypes
{
    //Значение без величины
    public class MomValue : Mom
    {
        internal MomValue(ErrMom err) : base(err) {}

        public override DataType DataType
        {
            get { return DataType.Value; }
        }

        public override bool Boolean
        {
            get { return false; }
            internal set { }
        }

        public override int Integer
        {
            get { return 0; }
            internal set { }
        }

        public override double Real
        {
            get { return 0; }
            internal set { }
        }

        public override DateTime Date
        {
            get { return Different.MinDate; }
            internal set { }
        }

        public override string String
        {
            get { return ""; }
            internal set { }
        }

        public override object Object
        {
            get { return 0; }
            internal set { }
        }

        public override bool ValueEquals(IMom mom)
        {
            return false;
        }

        public override bool ValueLess(IMom mom)
        {
            return false;
        }
        
        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return new MomValue(Error) {Time = time};
        }

        //Запись в рекордсет
        public override void ValueToRec(IRecordAdd rec, string field) { }
    }
}