using System;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый клас для мгновенных значений
    public abstract class Mom : CalcVal, IMom
    {
        protected Mom(ErrMom err) : base(err) {}
            
        //Время значения
        public DateTime Time { get; internal set; }

        //Возвращает себя
        public Mom ToMom { get { return this; } }
        //Количество значений
        public int Count { get { return 1; } }
        //Возвращает значение по номеру
        public Mom this[int n] { get { return n == 0 ? this : null; } }
        
        //Значения разных типов
        public abstract bool Boolean { get; internal set; }
        public abstract int Integer { get; internal set; }
        public abstract double Real { get; internal set; }
        public abstract DateTime Date { get; internal set; }
        public abstract string String { get; internal set; }
        public abstract object Object { get; internal set; }
        
        //Сравнение значений
        public abstract bool ValueEquals(IMom mom);
        public abstract bool ValueLess(IMom mom);
        //Сравнение значений и ошибок
        public bool ValueAndErrorEquals(IMom mom)
        {
            return ValueEquals(mom) && Error == mom.Error;
        }

        public abstract void ValueToRec(IRecordAdd rec, string field);

        //Создание
        //Из логического значения
        public static Mom Create(DateTime time, bool b, ErrMom err = null)
        {
            return new MomBoolean(err) {Time = time, Boolean = b};
        }
        public static Mom Create(bool b, ErrMom err = null)
        {
            return Create(Different.MinDate, b, err);
        }
        public static Mom Create(DataType dtype, DateTime time, bool b, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Boolean = b;
            return mom;
        }
        public static Mom Create(DataType dtype, bool b, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, b, err);
        }

        //Из целого значения
        public static Mom Create(DateTime time, int i, ErrMom err = null)
        {
            return new MomInteger(err) { Time = time, Integer = i};
        }
        public static Mom Create(int i, ErrMom err = null)
        {
            return Create(Different.MinDate, i, err);
        }
        public static Mom Create(DataType dtype, DateTime time, int i, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Integer = i;
            return mom;
        }
        public static Mom Create(DataType dtype, int i, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, i, err);
        }
        
        //Из действительного значения
        public static Mom Create(DateTime time, double r, ErrMom err = null)
        {
            return new MomInteger(err) { Time = time, Real = r };
        }
        public static Mom Create(double r, ErrMom err = null)
        {
            return Create(Different.MinDate, r, err);
        }
        public static Mom Create(DataType dtype, DateTime time, double r, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Real = r;
            return mom;
        }
        public static Mom Create(DataType dtype, double r, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, r, err);
        }

        //Из даты
        public static Mom Create(DateTime time, DateTime d, ErrMom err = null)
        {
            return new MomInteger(err) { Time = time, Date = d };
        }

        //Из строки
        public static Mom Create(DateTime time, string s, ErrMom err = null)
        {
            return new MomInteger(err) { Time = time, String = s};
        }
        public static Mom Create(string s, ErrMom err = null)
        {
            return Create(Different.MinDate, s, err);
        }
        public static Mom Create(DataType dtype, DateTime time, string s, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.String = s;
            return mom;
        }
        public static Mom Create(DataType dtype, string s, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, s, err);
        }

        //Из объекта
        public static Mom Create(DataType dtype, DateTime time, object ob, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Object = ob;
            return mom;
        }
        public static Mom Create(DataType dtype, object ob, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, ob, err);
        }

        //Без значения
        public static Mom Create(DataType dtype, DateTime time, ErrMom err = null)
        {
            return CreateMom(dtype, time, err);
        }
        public static Mom Create(DataType dtype, ErrMom err = null)
        {
            return Create(dtype, Different.MinDate, err);
        }

        private static Mom CreateMom(DataType dtype, DateTime time, ErrMom err)
        {
            Mom mom;
            switch (dtype)
            {
                case DataType.Real:
                    mom = new MomReal(err);
                    break;
                case DataType.Boolean:
                    mom = new MomBoolean(err);
                    break;
                case DataType.Integer:
                    mom = new MomInteger(err);
                    break;
                case DataType.String:
                    mom = new MomString(err);
                    break;
                case DataType.Time:
                    mom = new MomTime(err);
                    break;
                default:
                    mom = new MomValue(err);
                    break;
            }
            mom.Time = time;
            return mom;
        }

        //Клонирует значение
        public abstract Mom Clone(DateTime time);
        public Mom Clone() { return Clone(Time);}
    }
}
