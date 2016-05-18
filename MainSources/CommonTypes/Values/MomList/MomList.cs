using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    public abstract class MomList : Mean, IMomentsVal
    {
        //Количество значений
        public override int Count
        {
            get { return Times.Count; }
        }

        //Текущий номер значений
        public int CurNum { get; set; }
        
        //Текущее значение
        protected abstract IMom CurMom { get; }
        //Текущее добавляемое значение
        protected abstract IMom CurAddMom { get; }

        //Тип данных
        public override DataType DataType { get { return CurMom.DataType; } }

        //Заполнить текущее значение из списка по индексу
        protected abstract void GetCurMom(int i);
        //Добавить текущее значение для добавления в список по индексу или в конец
        protected abstract void AddCurMom(int i);
        protected abstract void AddCurMom();

        //Список времен
        private readonly List<DateTime> _times = new List<DateTime>();
        protected List<DateTime> Times { get { return _times; } }

        //Время по индексу
        DateTime IMomentsVal.Time(int i)
        {
            return Times[i];
        }
        //Время текущего значения
        public DateTime Time { get { return Times[CurNum]; }}

        //Словарь ошибок, ключи - номера значений в списке
        private DicI<ErrMom> _errors;
        protected DicI<ErrMom> Errors { get { return _errors ?? (_errors = new DicI<ErrMom>()); } }

        //Ошибка по индексу
        public ErrMom Err(int i)
        {
            if (_errors == null) return null;
            return Errors[i];
        }
        //Текущая ошибка
        public override ErrMom Error { get { return Err(CurNum); }  }

        //Получение значений по индексу
        public bool GetBoolean(int i)
        {
            GetCurMom(i);
            return CurMom.Boolean;
        }
        public int GetInteger(int i)
        {
            GetCurMom(i);
            return CurMom.Integer;
        }

        public double GetReal(int i)
        {
            GetCurMom(i);
            return CurMom.Real;
        }

        public DateTime GetDate(int i)
        {
            GetCurMom(i);
            return CurMom.Date;
        }

        public string GetString(int i)
        {
            GetCurMom(i);
            return CurMom.String;
        }

        //Добавление значений в список
        private void AddTimeErrorMean(DateTime time, ErrMom err, bool skipEquals)
        {
            if (Count == 0) 
                AddMomToEnd(time, err);
            else
            {
                var n = Count - 1;
                GetCurMom(n);
                if (time >= Times[n] && (!skipEquals || !CurAddMom.ValueAndErrorEquals(CurMom)))
                    AddMomToEnd(time, err);
                else 
                {
                    int i = n;
                    while (i >= 0 && Times[i] > time) i--;
                    i++;
                    Times.Insert(i, time);
                    AddCurMom(i);
                    if (err != null) Errors.Add(i, err);
                }
            }
        }

        private void AddMomToEnd(DateTime time, ErrMom err)
        {
            Times.Add(time);
            AddCurMom();
            if (err != null) Errors.Add(Count - 1, err);
        }

        public void AddMom(IMom mom, bool skipEquals = false)
        {
            CurAddMom.GetValueFromMean(mom);
            AddTimeErrorMean(mom.Time, mom.Error, skipEquals);
        }

        public void AddMom(DateTime time, bool b, ErrMom err = null, bool skipEquals = false)
        {
            CurAddMom.Boolean = b;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, int i, ErrMom err = null, bool skipEquals = false)
        {
            CurMom.Integer = i;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, double r, ErrMom err = null, bool skipEquals = false)
        {
            CurMom.Real = r;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, DateTime d, ErrMom err = null, bool skipEquals = false)
        {
            CurMom.Date = d;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, string s, ErrMom err = null, bool skipEquals = false)
        {
            CurMom.String = s;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            CurMom.ValueToRec(rec, field);
        }
        public override IMom Clone(DateTime time)
        {
            return CurMom.Clone(time);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return CurMom.Clone(time, err);
        }
    }
}