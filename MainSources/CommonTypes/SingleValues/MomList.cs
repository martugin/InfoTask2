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
        protected abstract Mean CurMean { get; }
        //Тип данных
        public override DataType DataType { get { return CurMean.DataType; } }

        //Заполнить текущее значение из списка по индексу
        protected abstract void GetCurMean(int i);
        //Добавить текущее значение в список индексу или в конец
        protected abstract void AddCurMean(int i);
        protected abstract void AddCurMean();

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

        //Список ошибок с указанием времен
        private List<MeanErrTime> _errors;
        protected List<MeanErrTime> Errors { get { return _errors ?? (_errors = new List<MeanErrTime>()); } }

        //Ошибка по индексу
        public ErrMom Err(int i)
        {
            //Двоичный поиск
            throw new NotImplementedException();
        }
        //Текущая ошибка
        public override ErrMom Error { get { return Err(CurNum); }  }

        //Получение значений по индексу
        public bool GetBoolean(int i)
        {
            GetCurMean(i);
            return CurMean.Boolean;
        }
        public int GetInteger(int i)
        {
            GetCurMean(i);
            return CurMean.Integer;
        }

        public double GetReal(int i)
        {
            GetCurMean(i);
            return CurMean.Real;
        }

        public DateTime GetDate(int i)
        {
            GetCurMean(i);
            return CurMean.Date;
        }

        public string GetString(int i)
        {
            GetCurMean(i);
            return CurMean.String;
        }

        //Добавление значений в список
        private void AddTimeErrorMean(DateTime time, ErrMom err, bool skipEquals)
        {
            if (Times.Count == 0 || time >= Times[Times.Count - 1])
            {
                Times.Add(time);
                AddCurMean();
                if (err != null)
                    Errors.Add(new MeanErrTime(time, err));
            }
            else
            {
                //Двоичный поиск
                throw new NotImplementedException();
            }
        }

        public void AddMom(IMom mom, bool skipEquals = false)
        {
            CurMean.GetValueFromMean(mom);
            AddTimeErrorMean(mom.Time, mom.Error, skipEquals);
        }

        public void AddMom(DateTime time, bool b, ErrMom err = null, bool skipEquals = false)
        {
            CurMean.Boolean = b;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, int i, ErrMom err = null, bool skipEquals = false)
        {
            CurMean.Integer = i;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, double r, ErrMom err = null, bool skipEquals = false)
        {
            CurMean.Real = r;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, DateTime d, ErrMom err = null, bool skipEquals = false)
        {
            CurMean.Date = d;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public void AddMom(DateTime time, string s, ErrMom err = null, bool skipEquals = false)
        {
            CurMean.String = s;
            AddTimeErrorMean(time, err, skipEquals);
        }

        public override void ValueToRec(IRecordAdd rec, string field)
        {
            CurMean.ValueToRec(rec, field);
        }
        public override IMom Clone(DateTime time)
        {
            return CurMean.Clone(time);
        }
        public override IMom Clone(DateTime time, ErrMom err)
        {
            return CurMean.Clone(time, err);
        }
    }
}