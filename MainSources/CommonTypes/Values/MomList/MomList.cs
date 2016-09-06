using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Список мгновенных значений
    public abstract class MomList : CalcVal, IMomList
    {
        //Количество значений
        public int Count { get { return _times.Count; } }
        
        //Список времен
        private readonly List<DateTime> _times = new List<DateTime>();
        //Время i-ого значения
        public DateTime Time(int i) { return _times[i]; }

        //Список ошибок
        private List<ErrMom> _errors;
        //Ошибка i-ого значения
        public ErrMom Error(int i)
        {
            if (_errors == null) return null;
            return _errors[i];
        }

        //Мгновенное значение для добавления в список и получения из списка
        protected Mean BufMean { get; set; }
        //Тип данных
        public override DataType DataType { get { return BufMean.DataType; } }

        //Добавить буферное значение в список по индексу или в конец
        protected abstract void AddBufMom(int i);
        protected abstract void AddBufMomEnd();

        //Получить значение из списка по индексу
        public abstract Mean Mean(int i);

        //Добавить ошибку в i-ю позицию списка
        private void AddError(ErrMom err, int i)
        {
            if (err == null && _errors == null) return;
            if (_errors == null)
            {
                _errors = new List<ErrMom>();
                foreach (var t in _times)
                    _errors.Add(null);
                _errors[i] = err;
            }
            else _errors.Insert(i, err);
        }
        
        //Добавить время, ошибку и значение в списки
        private void AddTimeErrorMean(DateTime time, ErrMom err)
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

        private void AddMomToEnd(DateTime time, ErrMom err)
        {
            _times.Add(time);
            AddError(err, Count - 1);
            AddBufMomEnd();
        }

        //Добавление значений в список, возвращают количество реально добавленных значений
        public void AddMom(IMom mom)
        {
            BufMean.CopyValueFrom(mom);
            AddTimeErrorMean(mom.Time, mom.Error);
        }

        public void AddMom(DateTime time, IMean mean)
        {
            BufMean.CopyValueFrom(mean);
            AddTimeErrorMean(time, mean.Error);
        }

        public void AddMom(DateTime time, bool b, ErrMom err = null)
        {
            BufMean.Boolean = b;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, int i, ErrMom err = null)
        {
            BufMean.Integer = i;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, double r, ErrMom err = null)
        {
            BufMean.Real = r;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, DateTime d, ErrMom err = null)
        {
            BufMean.Date = d;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, string s, ErrMom err = null)
        {
            BufMean.String = s;
            AddTimeErrorMean(time, err);
        }

        public void AddMom(DateTime time, object ob, ErrMom err = null)
        {
            BufMean.Object = ob;
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

        //Получение значений по индексу
        public bool Boolean(int i)
        {
            return Mean(i).Boolean;
        }
        public int Integer(int i)
        {
            return Mean(i).Integer;
        }

        public double Real(int i)
        {
            return Mean(i).Real;
        }

        public DateTime Date(int i)
        {
            return Mean(i).Date;
        }

        public string String(int i)
        {
            return Mean(i).String;
        }

        //Запись значения в рекордсет rec, поле field
        public void ValueToRec(IRecordAdd rec, string field, int i)
        {
            Mean(i).ValueToRec(rec, field);
        }

        public IMean CloneMean(int i)
        {
            return Mean(i).CloneMean(Error(i));
        }

        public IMean CloneMean(int i, ErrMom err)
        {
            return Mean(i).CloneMean(Error(i).Add(err));
        }

        //Клонирование текущего значения
        public IMom CloneMom(int i)
        {
            return Mean(i).CloneMom(Time(i), Error(i));
        }

        public IMom CloneMom(int i, ErrMom err)
        {
            return Mean(i).CloneMom(Time(i), Error(i).Add(err));
        }

        public IMom CloneMom(int i, DateTime time)
        {
            return Mean(i).CloneMom(time, Error(i));
        }

        public IMom CloneMom(int i, DateTime time, ErrMom err)
        {
            return Mean(i).CloneMom(time, Error(i).Add(err));
        }
        
        //Последнее значение
        public IMean LastMean
        {
            get
            {
                if (Count == 0) return null;
                return CloneMom(_times.Count - 1);
            }
        }

        //Итоговая ошибка
        public override ErrMom TotalError
        {
            get
            {
                ErrMom terr = null;
                if (_errors != null)
                    foreach (var err in _errors)
                        terr = terr.Add(err);
                return terr;
            }
        }

        //Todo Подумать, что сделать с интерполяций
        //Интерполяция типа type значений list на время time по точке с номером n и следующим за ней, если n = -1, то значение в начале
        //public IMom Interpolation(InterpolationType type, int n, DateTime time)
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