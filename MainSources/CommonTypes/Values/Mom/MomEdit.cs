using System;
using BaseLibrary;

namespace CommonTypes
{
    //Мгновенные значения с возможностью изменения, декоратор над Mean
    public class MomEdit: CalcVal, IMom
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
        //Добавить ошибку в значение, возвращает себя
        public MomEdit AddError(ErrMom err)
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
            return _mean.ValueEquals(mean) && Error == mean.Error;
        }

        //Запись в рекордсет
        public void ValueToRec(IRecordAdd rec, string field)
        {
            _mean.ValueToRec(rec, field);
        }

        //Копирует значение из другого мгновенного значения, возвращает себя
        public MomEdit CopyValueFrom(IMean mean)
        {
            _mean.CopyValueFrom(mean);
            return this;
        }
        //Копирует время, ошибку и значение из другого IMom, возвращает себя
        public MomEdit CopyAllFrom(IMom mom)
        {
            _mean.CopyValueFrom(mom);
            Time = mom.Time;
            Error = mom.Error;
            return this;
        }
        //Копирует значение из списка мгновенных значений по указанной позиции
        public MomEdit CopyValueFrom(IMomListRead list, //список значений
                                                        int i) //Позиция
        {
            var buf = list.MeanI(i);
            _mean.CopyValueFrom(buf);
            return this;
        }
        //Копирует время, ошибку и значение из списка мгновенных значений по указанной позиции
        public MomEdit CopyAllFrom(IMomListRead list, //список значений
                                                    int i) //Позиция
        {
            var buf = list.MeanI(i);
            _mean.CopyValueFrom(buf);
            Time = list.TimeI(i);
            Error = list.ErrorI(i);
            return this;
        }

        //Принимает значение по умолчанию
        public MomEdit MakeDefaultValue()
        {
            _mean.MakeDefaultValue();
            return this;
        }

        //Создание нового значения на основе этого
        public IMean CloneMean()
        {
            return _mean.CloneMean();
        }
        public IMean CloneMean(ErrMom err)
        {
            return _mean.CloneMean(err);
        }
        public IMom CloneMom()
        {
            return _mean.CloneMom(Time, Error);
        }
        public IMom CloneMom(ErrMom err)
        {
            return _mean.CloneMom(Time, err);
        }
        public IMom CloneMom(DateTime time)
        {
            return _mean.CloneMom(time, Error);
        }
        public IMom CloneMom(DateTime time, ErrMom err)
        {
            return _mean.CloneMom(time, err);
        }

        //Типы данных и значения
        public override DataType DataType { get { return _mean.DataType; } }
        public override ICalcVal CalcValue { get { return this; } }
        public override ErrMom TotalError { get { return Error; } }
        public int Count { get { return 1; } }
        public IMean LastMean { get { return this; } }

        //Методы из MomList
        public ErrMom ErrorI(int i) {return Error; }
        public bool BooleanI(int i) { return Boolean; }
        public int IntegerI(int i) { return Integer; }
        public double RealI(int i) { return Real; }
        public DateTime DateI(int i) { return Date; }
        public string StringI(int i) { return String;  }
        public void ValueToRecI(IRecordAdd rec, string field, int i) { ValueToRec(rec, field); }
        public IMean CloneMeanI(int i) { return CloneMean(); }
        public IMean CloneMeanI(int i, ErrMom err) { return CloneMean(err); }
        public IMom CloneMomI(int i, DateTime time) { return CloneMom(time); }
        public IMom CloneMomI(int i, DateTime time, ErrMom err) { return CloneMom(time, err); }
        public IMean MeanI(int i) { return this; }
        public DateTime TimeI(int i) { return Time; }
        public IMom CloneMomI(int i) { return CloneMom(); }
        public IMom CloneMomI(int i, ErrMom err) { return CloneMom(err); }
    }
}
