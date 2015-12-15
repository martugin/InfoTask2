using System;
using BaseLibrary;

namespace CommonTypes
{
    //мгновенное значение - время
    public class MomTime :Mom 
    {
         public MomTime(ErrMom err) : base(err) { }

         //Значение
         private DateTime _date;

         //Тип данных
         public override DataType DataType
         {
             get { return DataType.Time; }
         }

         public override bool Boolean
         {
             get { return false; }
             internal set { _date = Different.MinDate; }
         }

         public override int Integer
         {
             get { return 0; }
             internal set { _date = Different.MinDate; }
         }

         public override double Real
         {
             get { return 0; }
             internal set { _date = Different.MinDate; }
         }

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

         //Сравнение значений
         public override bool ValueEquals(IMom mom)
         {
             return _date == mom.Date;
         }
         public override bool ValueLess(IMom mom)
         {
             return _date < mom.Date;
         }

         //Копия значения
         public override Mom Clone(DateTime time)
         {
             return Create(time, Date, Error);
         }

         //Запись в рекордсет
         public override void ValueToRec(IRecordAdd rec, string field)
         {
             rec.Put(field, Time);
         }
    }
}