using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mom
    public class MomEdit: Val, IMom
    {
        public MomEdit(DataType dtype, DateTime time, ErrMom err = null)
        {
            _mom = (Mean)MomFactory.NewMom(dtype, time, err);
        }

        //Обертываемое мгновенное значение
        private readonly Mean _mom;
        public IMom Mom { get { return (IMom)_mom; }}

        //Время значения
        public DateTime Time { get { return ((IMom)_mom).Time; } }

        //Ошибка
        public ErrMom Error
        {
            get { return _mom.Error; }
            set { _mom.Error = value; }
        }
        //Добавить новую ошибку
        public void AddError(ErrMom err)
        {
            Error = Error.Add(err);
        }

        //Значения разных типов 
        public bool Boolean
        {
            get { return _mom.Boolean; }
            set { _mom.Boolean = value; }
        }
        public int Integer
        {
            get { return _mom.Integer; }
            set { _mom.Integer = value; }
        }
        public double Real
        {
            get { return _mom.Real; }
            set { _mom.Real = value; }
        }
        public DateTime Date
        {
            get { return _mom.Date; }
            set { _mom.Date = value; }
        }
        public string String
        {
            get { return _mom.String; }
            set { _mom.String = value;}
        }
        public object Object
        {
            get { return _mom.Object; }
            internal set { _mom.Object = value; }
        }

        public bool ValueEquals(IMean mean)
        {
            return _mom.ValueEquals(mean);
        }

        public bool ValueLess(IMean mean)
        {
            return _mom.ValueLess(mean);
        }

        public bool ValueAndErrorEquals(IMean mean)
        {
            return _mom.ValueAndErrorEquals(mean);
        }

        //Запись в рекордсет
        public void ValueToRec(IRecordAdd rec, string field)
        {
            _mom.ValueToRec(rec, field);
        }

        //Копирует значение из другого мгновенного значения
        public void CopyValueFrom(IMean mean)
        {
            switch (mean.DataType)
            {
                case DataType.Real:
                    Real = mean.Real;
                    break;
                case DataType.Integer:
                    Integer = mean.Integer;
                    break;
                case DataType.Boolean:
                    Boolean = mean.Boolean;
                    break;
                case DataType.String:
                    String = mean.String;
                    break;
                case DataType.Time:
                    Date = mean.Date;
                    break;
            }
        }

        public IMom Clone(DateTime time)
        {
            return _mom.Clone(time);
        }

        public IMom Clone(DateTime time, ErrMom err)
        {
            return _mom.Clone(time, err);
        }

        //Типы данных и значения
        public override DataType DataType { get { return _mom.DataType; } }
        public override ICalcVal CalcValue { get { return _mom.CalcValue; } }
        public ErrMom TotalError { get { return _mom.Error; } }
        public int Count { get { return 1; } }
    }
}
