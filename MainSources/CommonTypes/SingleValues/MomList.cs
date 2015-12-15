using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BaseLibrary;

namespace CommonTypes
{
    //Список мгновенных значений 
    public class MomList : CalcVal, IMomentsVal
    {
        public MomList(DataType dataType, ErrMom err = null) : base(err)
        {
            _dataType = dataType;
            _moments = new List<Mom>();
            Moments = new ReadOnlyCollection<Mom>(_moments);
        }

        //Тип данных
        private readonly DataType _dataType;
        public override DataType DataType { get { return _dataType; } }

        //Список мгновенных значений
        private readonly List<Mom> _moments;
        public ReadOnlyCollection<Mom> Moments { get; private set; }
        //Количество значений
        public int Count { get { return Moments.Count; } }
        //Возвращает значение по номеру
        public Mom this[int n] { get { return Moments[n]; }}

        //Добавляет мгновенное значение в список, сохраняет упорядоченность по времени, два момента списка могут иметь одинаковое время
        //skipEquals - не добавлять значение, если оно совпадает с предыдущим
        public Mom AddMom(Mom mom, bool skipEquals = false)
        {
            if (mom == null || !mom.DataType.LessOrEquals(DataType))
                return null;
            return AddMomWithoutCheck(mom, skipEquals);
        }

        //Добавка значения в список без проверки типа данных
        private Mom AddMomWithoutCheck(Mom mom, bool skipEquals)
        {
            if (_moments.Count == 0 || mom.Time >= _moments[Count - 1].Time)
            {
                if (!skipEquals || _moments.Count == 0 || !_moments[Count - 1].ValueAndErrorEquals(mom))
                    _moments.Add(mom);
                else return null;
            }
            else
            {
                int i = _moments.Count - 1;
                while (i >= 0 && _moments[i].Time > mom.Time) i--;
                if (!skipEquals || i == -1 || !_moments[i].ValueAndErrorEquals(mom))
                    _moments.Insert(i + 1, mom);
                else return null;
            }
            return mom;
        }

        //Добавляет клон MomEdit в список
        public Mom AddMomEdit(MomEdit edit, bool skipEquals = false)
        {
            return AddMom(edit.ToMom, skipEquals);
        }

        //Создание нового Mom и добавление его в MomList
        //error - ошибка, skipEquals - не добавлять повторяющееся значение
        public Mom AddMom(DateTime time, bool b, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, b, error), skipEquals);
        }
        public Mom AddMom(DateTime time, int i, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, i, error), skipEquals);
        }
        public Mom AddMom(DateTime time, double r, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, r, error), skipEquals);
        }
        public Mom AddMom(DateTime time, DateTime d, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, d, error), skipEquals);
        }
        public Mom AddMom(DateTime time, string s, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, s, error), skipEquals);
        }
        public Mom AddMom(DateTime time, object ob, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, ob, error), skipEquals);
        }
        public Mom AddMom(DateTime time, ErrMom error = null, bool skipEquals = false)
        {
            return AddMomWithoutCheck(Mom.Create(DataType, time, error), skipEquals);
        }

        //Очищает список значений
        public void Clear()
        {
            _moments.Clear();
        }

        //Возвращает последнее мгновенное значение, или null, если список пустой
        public Mom ToMom
        {
            get
            {
                if (_moments.Count == 0) return null;
                return _moments[_moments.Count - 1];
            }
        }

        //Итоговая ошибка для записи в Result
        public override ErrMom TotalError 
        { 
            get
            {
                ErrMom err = Error;
                foreach (var mom in Moments)
                    err = err.Add(mom.Error);
                return err;
            } 
        }

        //Интерполяция типа type значений list на время time по точке с номером n и следующим за ней, если n = -1, то значение в начале
        public Mom Interpolation(InterpolationType type, int n, DateTime time)
        {
            if (Count == 0) return Mom.Create(DataType.Value, time);
            if (n >= 0 && time == Moments[n].Time) 
                return Moments[n];
            if (type == InterpolationType.Constant || (DataType != DataType.Real && DataType != DataType.Time) || n < 0 || n >= Moments.Count - 1)
                return Moments[n < 0 ? 0 : n].Clone(time);

            var err = Moments[n].Error.Add(Moments[n + 1].Error);
            double t = time.Minus(Moments[n].Time);
            double t0 = Moments[n + 1].Time.Minus(Moments[n].Time);
            if (DataType == DataType.Real)
            {
                double x1 = Moments[n + 1].Real;
                double x0 = Moments[n].Real;
                double r = t0 == 0 || t == 0 ? x0 : x0 + t * (x1 - x0) / t0;
                return Mom.Create(time, r, err);
            }
            DateTime x1D = Moments[n + 1].Date;
            DateTime x0D = Moments[n].Date;
            DateTime d = t0 == 0 || t == 0 ? x0D : x0D.AddSeconds(t * x1D.Minus(x0D) / t0);
            return Mom.Create(time, d, err);
        }

        //Выделяет часть списка за указаный период
        public MomList GetPart(DateTime beg, DateTime en, bool addBegin, InterpolationType interpolation = InterpolationType.Constant)
        {
            var res = new MomList(DataType, Error);
            if (Moments.Count == 0) return res;
            int n = -1;
            while (n + 1 < Moments.Count && Moments[n + 1].Time < beg)
                n++;
            if (addBegin && Moments[n].Time != beg) 
                res.AddMom(Interpolation(interpolation, n, beg));
            while (Moments[n].Time <= en)
                res.AddMom(Moments[n]);
            return res;
        }
    }
}