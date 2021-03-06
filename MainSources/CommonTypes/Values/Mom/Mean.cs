﻿using System;
using BaseLibrary;

namespace CommonTypes
{
    //Одно значение
    public abstract class Mean : CalcVal, IMean, IReadMean
    {
        public virtual bool Boolean
        {
            get { return false; }
            set { }
        }
        public virtual int Integer
        {
            get { return 0; }
            set { }
        }
        public virtual double Real
        {
            get { return 0; }
            set { }
        }
        public virtual DateTime Date
        {
            get { return Static.MinDate; }
            set { }
        }
        public virtual string String
        {
            get { return ""; }
            set { }
        }
        public virtual object Object
        {
            get { return 0; }
            set { }
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
            set { }
        }
        public MomErr ErrorI(int i) { return Error; }

        public virtual DateTime Time
        {
            get { return Static.MinDate; }
            set {}
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

        public bool ValueEquals(IBaseMean mean)
        {
            var rmean = (IReadMean) mean;
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.Real:
                case DataType.Weighted:
                    return Real == rmean.Real;
                case DataType.String:
                    return String == rmean.String;
                case DataType.Integer:
                    return Integer == rmean.Integer;
                case DataType.Boolean:
                    return Boolean == rmean.Boolean;
                case DataType.Time:
                    return Date == rmean.Date;
            }
            return false;
        }

        public bool ValueLess(IBaseMean mean)
        {
            var rmean = (IReadMean)mean;
            var dt = DataType.Add(mean.DataType);
            switch (dt)
            {
                case DataType.Real:
                case DataType.Weighted:
                    return Real < rmean.Real;
                case DataType.String:
                    return String.CompareTo(rmean.String) < 0;
                case DataType.Integer:
                    return Integer < rmean.Integer;
                case DataType.Boolean:
                    return !Boolean && rmean.Boolean;
                case DataType.Time:
                    return Date < rmean.Date;
            }
            return false;
        }

        public bool ValueAndErrorEquals(IBaseMean mean)
        {
            return ValueEquals(mean) && Error == ((IReadMean)mean).Error;
        }

        public abstract void ValueToRec(IRecordAdd rec, string field);
        public void ValueToRecI(int i, IRecordAdd rec, string field) { ValueToRec(rec, field); }
        public abstract void ValueFromRec(IRecordRead rec, string field);

        public abstract IReadMean ToMean();
        public abstract IReadMean ToMom(DateTime time);
        public abstract IReadMean ToMom(DateTime time, MomErr err);
        public IReadMean ToMom() { return ToMom(Time, Error);}
        public IReadMean ToMom(MomErr err) { return ToMom(Time, err); }

        public IReadMean ToMeanI(int i) { return ToMean(); }
        public IReadMean ToMomI(int i) { return ToMom(); }
        public IReadMean ToMomI(int i, MomErr err) { return ToMom(err); }
        public IReadMean ToMomI(int i, DateTime time) { return ToMom(time); }
        public IReadMean ToMomI(int i, DateTime time, MomErr err) { return ToMom(time, err); }

        public override MomErr TotalError
        {
            get { return Error; }
        }

        public virtual int Count { get { return 1; } }
        public virtual IReadMean LastMom { get { return this; } }

        //Скопировать значение из другого значения
        internal abstract void CopyValueFrom(IReadMean mean);
        //Присвоить значение по умолчанию
        internal abstract void MakeDefaultValue();

        //Клонировать
        public override object Clone()
        {
            return ToMean();
        }

        //Добавить мгновенное значение
        public void AddMom(IReadMean mom)
        {
            CopyValueFrom(mom);
            Time = mom.Time;
            Error = mom.Error;
        }
        public void AddMom(DateTime time, IReadMean mean, MomErr err = null)
        {
            CopyValueFrom(mean);
            Time = time;
            Error = err;
        }
        public void AddMom(DateTime time, bool b, MomErr err = null)
        {
            Boolean = b;
            Time = time;
            Error = err;
        }

        public void AddMom(DateTime time, int i, MomErr err = null)
        {
            Integer = i;
            Time = time;
            Error = err;
        }

        public void AddMom(DateTime time, double r, MomErr err = null)
        {
            Real = r;
            Time = time;
            Error = err;
        }

        public void AddMom(DateTime time, DateTime d, MomErr err = null)
        {
            Date = d;
            Time = time;
            Error = err;
        }

        public void AddMom(DateTime time, string s, MomErr err = null)
        {
            String = s;
            Time = time;
            Error = err;
        }

        public void AddMom(DateTime time, object ob, MomErr err = null)
        {
            Object = ob;
            Time = time;
            Error = err;
        }

        public void Clear() { }
    }
}