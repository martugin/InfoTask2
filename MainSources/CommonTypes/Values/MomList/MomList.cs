using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Список мгновенных значений
    public abstract class MomList : CalcVal, IMean
    {
        //Тип данных
        public override DataType DataType { get { return BufMom.DataType; } }

        //Количество значений
        public int Count { get { return _times.Count; } }
        
        //Список времен
        private readonly List<DateTime> _times = new List<DateTime>();
        //Список ошибок
        private List<MomErr> _errors;

        //Мгновенное значение для добавления в список и получения из списка
        protected Mean BufMom { get; set; }
        //Записать i-е значение в BufMom
        protected abstract void SetBufMom(int i);
        //Записать BufMom в i-е значение
        protected abstract void SaveBufMom(int i);

        //Получить значение из списка по индексу
        public IMean MeanI(int i)
        {
            SetBufMom(i);
            return BufMom;
        }

        //Текущий номер значения и время слдующего значения, для прохода по списку
        public int CurNum { get; set; }
        public DateTime NextTime 
        { 
            get
            {
                if (CurNum >= Count - 1) return Static.MaxDate;
                return TimeI(CurNum + 1);
            } 
        }

        //Время i-ого значения
        public DateTime TimeI(int i) { return _times[i]; }
        //Время значения с номером CurNum
        public DateTime Time
        {
            get { return _times[CurNum]; }
            set { _times[CurNum] = value; }
        }

        //Ошибка i-ого значения
        public MomErr ErrorI(int i)
        {
            if (_errors == null) return null;
            return _errors[i];
        }
        //Ошибка значения с номером CurNum
        public MomErr Error
        {
            get { return _errors[CurNum]; }
            set { _errors[CurNum] = value; }
        }

        //Значение по индексу
        public bool BooleanI(int i) { return MeanI(i).Boolean;}
        public int IntegerI(int i) { return MeanI(i).Integer; }
        public double RealI(int i) { return MeanI(i).Real; }
        public DateTime DateI(int i) { return MeanI(i).Date; }
        public string StringI(int i) { return MeanI(i).String; }
        public object ObjectI(int i) { return MeanI(i).Object; }

        //Значение по CurNum
        public bool Boolean
        {
            get { return MeanI(CurNum).Boolean; }
            set
            {
                BufMom.Boolean = value;
                SaveBufMom(CurNum);
            }
        }

        public int Integer
        {
            get { return MeanI(CurNum).Integer; }
            set
            {
                BufMom.Integer = value;
                SaveBufMom(CurNum);
            }
        }

        public double Real
        {
            get { return MeanI(CurNum).Real; }
            set
            {
                BufMom.Real = value;
                SaveBufMom(CurNum);
            }
        }

        public DateTime Date
        {
            get { return MeanI(CurNum).Date; }
            set
            {
                BufMom.Date = value;
                SaveBufMom(CurNum);
            }
        }

        public string String
        {
            get { return MeanI(CurNum).String; }
            set
            {
                BufMom.String = value;
                SaveBufMom(CurNum);
            }
        }

        public object Object
        {
            get { return MeanI(CurNum).Object; }
            set
            {
                BufMom.Object = value;
                SaveBufMom(CurNum);
            }
        }

        //Сравнение значений и ошибок
        public bool ValueEquals(IMean mean)
        {
            return MeanI(CurNum).ValueEquals(mean);
        }
        public bool ValueLess(IMean mean)
        {
            return MeanI(CurNum).ValueLess(mean);
        }
        public bool ValueAndErrorEquals(IMean mean)
        {
            return MeanI(CurNum).ValueAndErrorEquals(mean);
        }

        //Запись значения в рекордсет rec, поле field
        public void ValueToRecI(int i, IRecordAdd rec, string field)
        {
            MeanI(i).ValueToRec(rec, field);
        }
        public void ValueToRec(IRecordAdd rec, string field)
        {
            MeanI(CurNum).ValueToRec(rec, field);
        }
        public void ValueFromRec(IRecordRead rec, string field)
        {
            BufMom.ValueFromRec(rec, field);
            SaveBufMom(CurNum);
        }
        
        //Добавить буферное значение в список по индексу или в конец
        protected abstract void AddBufMom(int i);
        protected abstract void AddBufMomEnd();

        //Добавить ошибку в i-ю позицию списка
        private void AddError(MomErr err, int i)
        {
            if (err == null && _errors == null) return;
            if (_errors == null)
            {
                _errors = new List<MomErr>();
                foreach (var t in _times)
                    _errors.Add(null);
                _errors[i] = err;
            }
            else _errors.Insert(i, err);
        }
        
        //Добавить время, ошибку и значение в списки
        private void AddTimeErrorMean(DateTime time, MomErr err)
        {
            int num = Count - 1;
            if (Count == 0)
                AddMomToEnd(time, err);
            else if (time >= _times[num])
                AddMomToEnd(time, err);
            else
            {
                while (num >= 0 && _times[num] > time) num--;
                num++;
                _times.Insert(num, time);
                AddBufMom(num);
                AddError(err, num);    
            }
        }

        private void AddMomToEnd(DateTime time, MomErr err)
        {
            _times.Add(time);
            AddError(err, Count - 1);
            AddBufMomEnd();
        }

        //Добавление мгновенного значения
        public void AddMom(IMean mom)
        {
            BufMom.CopyValueFrom(mom);
            AddTimeErrorMean(mom.Time, mom.Error);
        }

        public void AddMom(DateTime time, IMean mean)
        {
            BufMom.CopyValueFrom(mean);
            AddTimeErrorMean(time, mean.Error);
        }

        public void AddMom(IMean mom, MomErr err)
        {
            BufMom.CopyValueFrom(mom);
            AddTimeErrorMean(mom.Time, err);
        }

        public void AddMom(DateTime time, IMean mean, MomErr err)
        {
            BufMom.CopyValueFrom(mean);
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, bool b, MomErr err = null)
        {
            BufMom.Boolean = b;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, int i, MomErr err = null)
        {
            BufMom.Integer = i;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, double r, MomErr err = null)
        {
            BufMom.Real = r;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, DateTime d, MomErr err = null)
        {
            BufMom.Date = d;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, string s, MomErr err = null)
        {
            BufMom.String = s;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, object ob, MomErr err = null)
        {
            BufMom.Object = ob;
            AddTimeErrorMean(time, err);
        }

        public void Clear()
        {
            _times.Clear();
            _errors = null;
            ClearMeans();
        }
        //Очистка самих значений
        protected abstract void ClearMeans();

        public IMean ToMeanI(int i)
        {
            return MeanI(i).ToMean();
        }
        
        public IMean ToMomI(int i)
        {
            return MeanI(i).ToMom(TimeI(i), ErrorI(i));
        }

        public IMean ToMomI(int i, MomErr err)
        {
            return MeanI(i).ToMom(TimeI(i), ErrorI(i).Add(err));
        }

        public IMean ToMomI(int i, DateTime time)
        {
            return MeanI(i).ToMom(time, ErrorI(i));
        }

        public IMean ToMomI(int i, DateTime time, MomErr err)
        {
            return MeanI(i).ToMom(time, ErrorI(i).Add(err));
        }

        public IMean ToMean() { return ToMeanI(CurNum);}
        public IMean ToMom() { return ToMomI(CurNum); }
        public IMean ToMom(MomErr err) { return ToMomI(CurNum, err); }
        public IMean ToMom(DateTime time) { return ToMomI(CurNum, time); }
        public IMean ToMom(DateTime time, MomErr err) { return ToMomI(CurNum, time, err); }
        
        //Последнее значение
        public IMean LastMom
        {
            get
            {
                if (Count == 0) return null;
                return ToMomI(_times.Count - 1);
            }
        }
        
        //Итоговая ошибка
        public override MomErr TotalError
        {
            get
            {
                if (_errors == null) return null;
                MomErr terr = null;
                foreach (var err in _errors)
                    terr = terr.Add(err);
                return terr;
            }
        }

        //Клонировать
        public override object Clone()
        {
            var mlist = MFactory.NewList(DataType);
            for (int i = 0; i < _times.Count; i++)
            {
                SetBufMom(i);
                mlist.AddMom(BufMom);
            }
            return mlist;
        }

        //Todo Подумать, что сделать с интерполяций
        //Интерполяция типа type значений list на время time по точке с номером n и следующим за ней, если n = -1, то значение в начале
        //public IMean Interpolation(InterpolationType type, int n, DateTime time)
        //{
        //    if (Count == 0) return MFactory.NewMom(DataType.Value, time);
        //    if (n >= 0 && time == Moments[n].Time)
        //        return Moments[n];
        //    if (type == InterpolationType.Constant || (DataType != DataType.Real && DataType != DataType.Time) || n < 0 || n >= Moments.Count - 1)
        //        return Moments[n < 0 ? 0 : n].Clone(time);

        //    var err = Moments[n].Error.Add(Moments[n + 1].Error);
        //    double t = time.Minus(Moments[n].Time);
        //    double t0 = Moments[n + 1].Time.Minus(Moments[n].Time);
        //    if (DataType == DataType.Real)
        //    {
        //        double x1 = Moments[n + 1].Real;
        //        double x0 = Moments[n].Real;
        //        double r = t0 == 0 || t == 0 ? x0 : x0 + t * (x1 - x0) / t0;
        //        return MFactory.NewMom(DataType.Real, time, r, err);
        //    }
        //    DateTime x1D = Moments[n + 1].Date;
        //    DateTime x0D = Moments[n].Date;
        //    DateTime d = t0 == 0 || t == 0 ? x0D : x0D.AddSeconds(t * x1D.Minus(x0D) / t0);
        //    return MFactory.NewMom(DataType.Time, time, d, err);
        //}

        ////Выделяет часть списка за указаный период
        //public MomList GetPart(DateTime beg, DateTime en, bool addBegin, InterpolationType interpolation = InterpolationType.Constant)
        //{
        //    var res = new MomList(DataType, Error);
        //    if (Moments.Count == 0) return res;
        //    int n = -1;
        //    while (n + 1 < Moments.Count && Moments[n + 1].Time < beg)
        //        n++;
        //    if (addBegin && Moments[n].Time != beg)
        //        res.AddMom(Interpolation(interpolation, n, beg));
        //    while (Moments[n].Time <= en)
        //        res.AddMom(Moments[n]);
        //    return res;
        //}
    }
}