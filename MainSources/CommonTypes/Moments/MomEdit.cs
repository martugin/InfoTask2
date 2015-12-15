using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mom
    public class MomEdit: Val, IMom
    {
        public MomEdit(DataType type, ErrMom error = null)
        {
            _mom = Mom.Create(type, error);
        }

        public MomEdit(DataType type, DateTime time, ErrMom error = null)
        {
            _mom = Mom.Create(type, time, error);
        }

        //Обертываемое мгновенное значение
        private readonly Mom _mom;

        //Время значения
        public DateTime Time 
        {
            get { return _mom.Time; }
            set { _mom.Time = value; } 
        }

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
            set { _mom.Object = value; }
        }

        public bool ValueEquals(IMom mom)
        {
            return _mom.ValueEquals(mom);
        }

        public bool ValueLess(IMom mom)
        {
            return _mom.ValueLess(mom);
        }

        public bool ValueAndErrorEquals(IMom mom)
        {
            return _mom.ValueAndErrorEquals(mom);
        }

        //Запись в рекордсет
        public void ValueToRec(IRecordAdd rec, string field)
        {
            _mom.ValueToRec(rec, field);
        }

        //Копирует значение из другого мгновенного значения
        public void CopyValueFrom(Mom mom)
        {
            if (mom is MomReal) _mom.Real = mom.Real;
            else if (mom is MomInteger) _mom.Integer = mom.Integer;
            else if (mom is MomBoolean) _mom.Boolean = mom.Boolean;
            else if (mom is MomString) _mom.String = mom.String;
            else if (mom is MomTime) _mom.Date = mom.Date;
        }

        //Типы данных и значения
        public override DataType DataType { get { return _mom.DataType; } }
        public override ICalcVal CalcValue { get { return _mom.CalcValue; } }

        public ErrMom TotalError { get { return _mom.Error; } }
        public Mom ToMom { get { return _mom; } }
        public int Count { get { return 1; } }
        public Mom this[int n] { get { return n == 0 ? ToMom : null; } }
    }
}