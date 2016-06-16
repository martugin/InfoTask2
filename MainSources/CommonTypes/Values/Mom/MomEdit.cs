using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mean
    public class MomEdit: Val, IMom
    {
        public MomEdit(DataType dtype)
        {
            _mean = (Mean)MFactory.NewMean(dtype);
            Time = Different.MinDate;
        }
        public MomEdit(DataType dtype, DateTime time) : this(dtype)
        {
            Time = time;
        }
        public MomEdit(DataType dtype, ErrMom err) : this(dtype)
        {
            Error = err;
        }
        public MomEdit(DataType dtype, DateTime time, ErrMom err) : this(dtype, time)
        {
            Error = err;
        }

        //Обертываемое мгновенное значение
        private readonly Mean _mean;
        //Время
        public DateTime Time { get; set; }
        //Ошибка
        public ErrMom Error { get; set; }

        //Значения разных типов 
        public bool Boolean
        {
            get { return _mean.Boolean; }
            set { _mean.Boolean = value; }
        }
        public int Integer
        {
            get { return _mean.Integer; }
            set { _mean.Integer = value; }
        }
        public double Real
        {
            get { return _mean.Real; }
            set { _mean.Real = value; }
        }
        public DateTime Date
        {
            get { return _mean.Date; }
            set { _mean.Date = value; }
        }
        public string String
        {
            get { return _mean.String; }
            set { _mean.String = value;}
        }
        public object Object
        {
            get { return _mean.Object; }
            set { _mean.Object = value; }
        }

        public bool ValueEquals(IMean mean)
        {
            return _mean.ValueEquals(mean);
        }

        public bool ValueLess(IMean mean)
        {
            return _mean.ValueLess(mean);
        }

        public bool ValueAndErrorEquals(IMean mean)
        {
            return _mean.ValueAndErrorEquals(mean);
        }

        //Запись в рекордсет
        public void ValueToRec(IRecordAdd rec, string field)
        {
            _mean.ValueToRec(rec, field);
        }

        //Копирует значение из другого мгновенного значения
        public void CopyValueFrom(IMean mean)
        {
            _mean.CopyValueFrom(mean);
        }
        //Копирует время, ошибку и значение из другого IMom
        public void CopyAllFrom(IMom mom)
        {
            _mean.CopyValueFrom(mom);
            Time = mom.Time;
            Error = mom.Error;
        }

        public IMom Clone()
        {
            return _mean.Clone(Time, Error);
        }
        public IMom Clone(DateTime time)
        {
            return _mean.Clone(time, Error);
        }
        public IMom Clone(DateTime time, ErrMom err)
        {
            return _mean.Clone(time, err);
        }

        //Типы данных и значения
        public override DataType DataType { get { return _mean.DataType; } }
        public override ICalcVal CalcValue { get { return _mean.CalcValue; } }
        public ErrMom TotalError { get { return _mean.Error; } }
        public int Count { get { return 1; } }
        public IMean LastMean { get { return _mean; } }
    }
}
