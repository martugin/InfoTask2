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

        public bool BooleanI(int i) { return Boolean; }
        public int IntegerI(int i) { return Integer; }
        public double RealI(int i) { return Real; }
        public DateTime DateI(int i) { return Date; }
        public string StringI(int i) { return String; }
        public object ObjectI(int i) { return Object; }

        public virtual ErrMom Error
        {
            get { return null; }
            internal set { }
        }
        public ErrMom ErrorI(int i) { return Error; }

        public virtual DateTime Time
        {
            get { return Different.MinDate; }
            internal set {}
        }
        public DateTime TimeI(int i) { return Time; }

        public int CurNum { get; set; }
        public DateTime NextTime
        {
            get
            {
                if (CurNum <= -1) return Time;
                return Different.MaxDate; 
            }
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
        public void ValueToRecI(IRecordAdd rec, string field, int i) { ValueToRec(rec, field); }

        public abstract IMean CloneMean();
        public abstract IMean CloneMean(ErrMom err);
        public abstract IMean CloneMom(DateTime time);
        public abstract IMean CloneMom(DateTime time, ErrMom err);
        public IMean CloneMom() { return CloneMom(Time, Error);}
        public IMean CloneMom(ErrMom err) { return CloneMom(Time, err); }

        public IMean CloneMeanI(int i) { return CloneMean(); }
        public IMean CloneMeanI(int i, ErrMom err) { return CloneMean(err); }
        public IMean CloneMomI(int i) { return CloneMom(); }
        public IMean CloneMomI(int i, ErrMom err) { return CloneMom(err); }
        public IMean CloneMomI(int i, DateTime time) { return CloneMom(time); }
        public IMean CloneMomI(int i, DateTime time, ErrMom err) { return CloneMom(time, err); }

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
    }
}