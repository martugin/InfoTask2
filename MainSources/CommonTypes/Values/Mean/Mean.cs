using System;
using BaseLibrary;

namespace CommonTypes
{
    //Одно значение
    public abstract class Mean : CalcVal, IMean
    {
        public virtual bool Boolean
        {
            get { return false; }
            internal set { }
        }
        public virtual int Integer
        {
            get { return 0; }
            internal set { }
        }
        public virtual double Real
        {
            get { return 0; }
            internal set { }
        }
        public virtual DateTime Date
        {
            get { return Different.MinDate; }
            internal set { }
        }
        public virtual string String
        {
            get { return ""; }
            internal set { }
        }
        public virtual object Object
        {
            get { return 0; }
            internal set { }
        }

        public bool ValueEquals(IMean mean)
        {
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.Real:
                case DataType.Weighted:
                    return Real == mean.Real;
                case DataType.String:
                    return String == mean.String;
                case DataType.Integer:
                    return Integer == mean.Integer;
                case DataType.Boolean:
                    return Boolean == mean.Boolean;
                case DataType.Time:
                    return Date == mean.Date;
            }
            return false;
        }

        public bool ValueLess(IMean mean)
        {
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.Real:
                case DataType.Weighted:
                    return Real < mean.Real;
                case DataType.String:
                    return String.CompareTo(mean.String) < 0;
                case DataType.Integer:
                    return Integer < mean.Integer;
                case DataType.Boolean:
                    return !Boolean && mean.Boolean;
                case DataType.Time:
                    return Date < mean.Date;
            }
            return false;
        }

        public bool ValueAndErrorEquals(IMean mean)
        {
            return ValueEquals(mean) && Error == mean.Error;
        }

        public abstract void ValueToRec(IRecordAdd rec, string field);
        public abstract IMean CloneMean();
        public abstract IMean CloneMean(ErrMom err);
        public abstract IMom CloneMom(DateTime time);
        public abstract IMom CloneMom(DateTime time, ErrMom err);

        public virtual ErrMom Error
        {
            get { return null; }
            internal set { }
        }

        public override ErrMom TotalError
        {
            get { return Error; }
        }

        public virtual int Count { get { return 1; } }
        public virtual IMean LastMean { get { return this; } }

        //Скопировать значение из другого значения
        internal abstract void CopyValueFrom(IMean mean);
        //Присвоить значение по умолчанию
        internal abstract void MakeDefaultValue();

        //Методы из MomList
        public bool BooleanI(int i) { return Boolean; }
        public int IntegerI(int i) { return Integer; }
        public double RealI(int i) { return Real; }
        public DateTime DateI(int i) { return Date; }
        public string StringI(int i) { return String; }
        public ErrMom ErrorI(int i) { return Error; }
        public void ValueToRecI(IRecordAdd rec, string field, int i) { ValueToRec(rec, field);}
        public IMean CloneMeanI(int i) { return CloneMean();}
        public IMean CloneMeanI(int i, ErrMom err) { return CloneMean(err); }
        public IMom CloneMomI(int i, DateTime time) { return CloneMom(time); }
        public IMom CloneMomI(int i, DateTime time, ErrMom err) { return CloneMom(time, err);}
        public IMean MeanI(int i) { return this; }
    }
}