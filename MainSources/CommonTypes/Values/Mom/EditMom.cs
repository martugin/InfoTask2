﻿using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mean
    public class EditMom: CalcVal, IMean, IReadMean
    {
        public EditMom(DataType dtype)
        {
            _mean = MFactory.NewMean(dtype);
            Time = Static.MinDate;
        }
        public EditMom(DataType dtype, DateTime time) : this(dtype)
        {
            Time = time;
        }
        public EditMom(DataType dtype, MomErr err) : this(dtype)
        {
            Error = err;
        }
        public EditMom(DataType dtype, DateTime time, MomErr err) : this(dtype, time)
        {
            Error = err;
        }

        //Обертываемое мгновенное значение
        private readonly Mean _mean;
        //Время
        public DateTime Time { get; set; }
        //Ошибка
        public MomErr Error { get; set; }
        //Добавить ошибку в значение, возвращает себя
        public EditMom AddError(MomErr err)
        {
            Error = Error.Add(err);
            return this;
        }

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
            set { _mean.String = value ?? "";}
        }
        public object Object
        {
            get { return _mean.Object; }
            set { _mean.Object = value; }
        }

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
            return _mean.ValueEquals(mean);
        }

        public bool ValueLess(IBaseMean mean)
        {
            return _mean.ValueLess(mean);
        }

        public bool ValueAndErrorEquals(IBaseMean mean)
        {
            return _mean.ValueEquals(mean) && Error == ((IReadMean)mean).Error;
        }

        //Запись в рекордсет
        public void ValueToRec(IRecordAdd rec, string field)
        {
            _mean.ValueToRec(rec, field);
        }

        public void ValueFromRec(IRecordRead rec, string field)
        {
            throw new NotImplementedException();
        }

        //Копирует значение из другого мгновенного значения, возвращает себя
        public EditMom CopyValueFrom(IReadMean mean)
        {
            _mean.CopyValueFrom(mean);
            return this;
        }
        //Копирует время, ошибку и значение из другого IMean, возвращает себя
        public EditMom CopyAllFrom(IReadMean mom)
        {
            _mean.CopyValueFrom(mom);
            Time = mom.Time;
            Error = mom.Error;
            return this;
        }
        //Копирует значение из списка мгновенных значений по указанной позиции
        public EditMom CopyValueFrom(IReadMean list, //список значений
                                                        int i) //Позиция
        {
            list.CurNum = i;
            _mean.CopyValueFrom(list);
            return this;
        }
        //Копирует время, ошибку и значение из списка мгновенных значений по указанной позиции
        public EditMom CopyAllFrom(IReadMean list, //список значений
                                                    int i) //Позиция
        {
            list.CurNum = i;
            _mean.CopyValueFrom(list);
            Time = list.TimeI(i);
            Error = list.ErrorI(i);
            return this;
        }

        //Принимает значение по умолчанию
        public EditMom MakeDefault()
        {
            _mean.MakeDefaultValue();
            Error = null;
            return this;
        }

        //Создание нового значения на основе этого
        public IReadMean ToMean()
        {
            return _mean.ToMean();
        }
        public IReadMean ToMom()
        {
            return _mean.ToMom(Time, Error);
        }
        public IReadMean ToMom(MomErr err)
        {
            return _mean.ToMom(Time, err);
        }
        public IReadMean ToMom(DateTime time)
        {
            return _mean.ToMom(time, Error);
        }
        public IReadMean ToMom(DateTime time, MomErr err)
        {
            return _mean.ToMom(time, err);
        }

        //Клонировать
        public override object Clone()
        {
            return new EditMom(DataType, Time, Error).CopyValueFrom(this);
        }

        //Типы данных и значения
        public override DataType DataType { get { return _mean.DataType; } }
        public override ICalcVal CalcValue { get { return this; } }
        public override MomErr TotalError { get { return Error; } }
        public int Count { get { return 1; } }
        public IReadMean LastMom { get { return this; } }

        //Методы из MomList
        public MomErr ErrorI(int i) {return Error; }
        public bool BooleanI(int i) { return Boolean; }
        public int IntegerI(int i) { return Integer; }
        public double RealI(int i) { return Real; }
        public DateTime DateI(int i) { return Date; }
        public string StringI(int i) { return String;  }
        public object ObjectI(int i) { return Object; }
        public void ValueToRecI(int i, IRecordAdd rec, string field) { ValueToRec(rec, field); }
        public IReadMean ToMeanI(int i) { return ToMean(); }
        public IReadMean ToMomI(int i) { return ToMom(); }
        public IReadMean ToMomI(int i, MomErr err) { return ToMom(err); }
        public IReadMean ToMomI(int i, DateTime time) { return ToMom(time); }
        public IReadMean ToMomI(int i, DateTime time, MomErr err) { return ToMom(time, err); }
        public IMean MeanI(int i) { return this; }
        public DateTime TimeI(int i) { return Time; }

        //Добавление значений как будто в список
        public virtual void AddMom(IReadMean mom)
        {
            CopyAllFrom(mom);
        }
        public void AddMom(DateTime time, IReadMean mean)
        {
            CopyAllFrom(mean);
            Time = time;
        }
        public void AddMom(IReadMean mom, MomErr err)
        {
            CopyAllFrom(mom);
            Error = err;
        }
        public void AddMom(DateTime time, IReadMean mean, MomErr err)
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
