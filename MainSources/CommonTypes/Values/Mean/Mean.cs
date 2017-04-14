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
            get { return Static.MinDate; }
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

        public virtual MomErr Error
        {
            get { return null; }
            internal set { }
        }
        public MomErr ErrorI(int i) { return Error; }

        public virtual DateTime Time
        {
            get { return Static.MinDate; }
            internal set {}
        }
        public DateTime TimeI(int i) { return Time; }

        public int CurNum { get; set; }
        public DateTime NextTime
        {
            get
            {
                if (CurNum <= -1) return Time;
                return Static.MaxDate; 
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
        public void ValueToRecI(int i, IRecordAdd rec, string field) { ValueToRec(rec, field); }

        public abstract IMean ToMean();
        public abstract IMean ToMean(MomErr err);
        public abstract IMean ToMom(DateTime time);
        public abstract IMean ToMom(DateTime time, MomErr err);
        public IMean ToMom() { return ToMom(Time, Error);}
        public IMean ToMom(MomErr err) { return ToMom(Time, err); }

        public IMean ToMeanI(int i) { return ToMean(); }
        public IMean ToMeanI(int i, MomErr err) { return ToMean(err); }
        public IMean ToMomI(int i) { return ToMom(); }
        public IMean ToMomI(int i, MomErr err) { return ToMom(err); }
        public IMean ToMomI(int i, DateTime time) { return ToMom(time); }
        public IMean ToMomI(int i, DateTime time, MomErr err) { return ToMom(time, err); }

        public override MomErr TotalError
        {
            get { return Error; }
        }

        public virtual int Count { get { return 1; } }
        public virtual IMean LastMom { get { return this; } }

        //Скопировать значение из другого значения
        internal abstract void CopyValueFrom(IMean mean);
        //Присвоить значение по умолчанию
        internal abstract void MakeDefaultValue();

        //Клонировать
        public override object Clone()
        {
            return ToMean();
        }
    }
}