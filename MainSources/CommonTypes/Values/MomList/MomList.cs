﻿using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Список мгновенных значений
    public abstract class MomList : Mean, IMomList
    {
        //Количество значений
        public override int Count { get { return _times.Count; } }
        //Текущий номер значений
        public int CurNum { get; set; }

        //Список времен
        private readonly List<DateTime> _times = new List<DateTime>();
        //Время текущего значения
        public DateTime Time { get { return _times[CurNum]; } }
        //Время i-ого значения
        public DateTime GetTime(int i) { return _times[i]; }

        //Список ошибок
        private List<ErrMom> _errors;
        //Текущая ошибка
        public override ErrMom Error
        {
            get
            {
                if (_errors == null) return null;
                return _errors[CurNum];
            }
        }
        //Ошибка i-ого значения
        public ErrMom GetError(int i) { return _errors[i]; }

        //Мгновенное значение для добавления в список
        protected Mean CurMean { get; set; }
        //Тип данных
        public override DataType DataType { get { return CurMean.DataType; } }

        //Добавить ошибку в i-ю позицию списка
        private void AddError(ErrMom err, int i)
        {
            if (err == null && _errors == null) return;
            if (_errors == null)
            {
                _errors = new List<ErrMom>();
                foreach (var t in _times)
                    _errors.Add(null);
            }
            _errors[i] = err;
        }
        
        //Добавить текущее значение для добавления в список по индексу или в конец
        protected abstract void AddCurMom(int i);
        protected abstract void AddCurMomEnd();
        
        //Добавить время, ошибку и значение в списки
        private int AddTimeErrorMean(DateTime time, ErrMom err, bool skipRepeats)
        {
            if (Count == 0)
            {
                AddMomToEnd(time, err);
                return 1;
            }
            CurNum = Count - 1;
            if (time >= _times[CurNum] )
            {
                if (!skipRepeats || !CurMean.ValueEquals(this) || err != Error)
                {
                    AddMomToEnd(time, err);
                    return 1;
                }
                return 0;
            }
            while (CurNum >= 0 && _times[CurNum] > time) CurNum--;
            CurNum++;
            _times.Insert(CurNum, time);
            AddCurMom(CurNum);
            AddError(err, CurNum);
            return 1;
        }

        private void AddMomToEnd(DateTime time, ErrMom err)
        {
            _times.Add(time);
            AddCurMomEnd();
            if (err != null) AddError(err, Count - 1);
            CurNum = Count - 1;
        }

        //Добавление значений в список, возвращают количество реально добавленных значений
        public int AddMom(IMom mom, bool skipRepeats = false)
        {
            CurMean.CopyValueFrom(mom);
            return AddTimeErrorMean(mom.Time, mom.Error, skipRepeats);
        }

        public int AddMom(DateTime time, IMean mean, bool skipRepeats = false)
        {
            CurMean.CopyValueFrom(mean);
            return AddTimeErrorMean(time, mean.Error, skipRepeats);
        }

        public int AddMom(DateTime time, bool b, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.Boolean = b;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        public int AddMom(DateTime time, int i, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.Integer = i;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        public int AddMom(DateTime time, double r, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.Real = r;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        public int AddMom(DateTime time, DateTime d, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.Date = d;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        public int AddMom(DateTime time, string s, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.String = s;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        public int AddMom(DateTime time, object ob, ErrMom err = null, bool skipRepeats = false)
        {
            CurMean.Object = ob;
            return AddTimeErrorMean(time, err, skipRepeats);
        }

        //Очистка списка значений
        public void Clear()
        {
            _times.Clear();
            _errors = null;
            ClearMeans();
        }
        //Очистка самих значений
        protected abstract void ClearMeans();

        //Получение значений по индексу
        public bool GetBoolean(int i)
        {
            CurNum = i;
            CurMean.CopyValueFrom(this);
            return CurMean.Boolean;
        }
        public int GetInteger(int i)
        {
            CurNum = i;
            CurMean.CopyValueFrom(this);
            return CurMean.Integer;
        }

        public double GetReal(int i)
        {
            CurNum = i;
            CurMean.CopyValueFrom(this);
            return CurMean.Real;
        }

        public DateTime GetDate(int i)
        {
            CurNum = i;
            CurMean.CopyValueFrom(this);
            return CurMean.Date;
        }

        public string GetString(int i)
        {
            CurNum = i;
            CurMean.CopyValueFrom(this);
            return CurMean.String;
        }

        //Запись значения в рекордсет rec, поле field
        public override void ValueToRec(IRecordAdd rec, string field)
        {
            CurMean.CopyValueFrom(this);
            CurMean.ValueToRec(rec, field);
        }

        //Клонирование текущего значения
        public IMom Clone()
        {
            CurMean.CopyValueFrom(this);
            return CurMean.Clone(GetTime(CurNum), GetError(CurNum));
        }
        public override IMom Clone(DateTime time)
        {
            CurMean.CopyValueFrom(this);
            return CurMean.Clone(time, GetError(CurNum));
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            CurMean.CopyValueFrom(this);
            return CurMean.Clone(time, err);
        }

        //Клонирование текущего значения
        public IMom Clone(int i)
        {
            CurNum = i;
            return Clone();
        }
        public IMom Clone(int i, DateTime time)
        {
            CurNum = i;
            return Clone(time);
        }
        public IMom Clone(int i, DateTime time, ErrMom err)
        {
            CurNum = i;
            return Clone(time, err);
        }

        //Последнее значение
        public override IMean LastMean
        {
            get
            {
                if (Count == 0) return null;
                CurNum = Count - 1;
                return this;
            }
        }

        //Итоговая ошибка
        public override ErrMom TotalError
        {
            get
            {
                var terr = Error;
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