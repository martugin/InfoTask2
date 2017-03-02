using System;
using System.Collections.Generic;

namespace CommonTypes
{
    //Список логических значений
    public class BoolMomList : MomList
    {
        public BoolMomList()
        {
            BufMom = new BoolMean();
        }

        //Список значений
        private readonly List<bool> _means = new List<bool>();

        //Добавить буферное значение в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMom.Boolean);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMom.Boolean);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            BufMom.Boolean = _means[i];
        }
        
        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список целых значений
    public class IntMomList : MomList
    {
        public IntMomList()
        {
            BufMom = new IntMean();
        }

        //Список значений
        private readonly List<int> _means = new List<int>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMom.Integer);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMom.Integer);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            BufMom.Integer = _means[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных значений
    public class RealMomList : MomList
    {
        public RealMomList()
        {
            BufMom = new RealMean();
        }

        //Список значений
        private readonly List<double> _means = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMom.Real);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMom.Real);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            BufMom.Real = _means[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список строковых значений
    public class StringMomList : MomList
    {
        public StringMomList()
        {
            BufMom = new StringMean();
        }

        //Список значений
        private readonly List<string> _means = new List<string>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMom.String);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMom.String);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            BufMom.String = _means[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список значений - дат
    public class TimeMomList : MomList
    {
        public TimeMomList()
        {
            BufMom = new TimeMean();
        }

        //Список значений
        private readonly List<DateTime> _means = new List<DateTime>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMom.Date);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMom.Date);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            BufMom.Date = _means[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных взвешенных значений
    public class WeightedMomList : RealMomList
    {
        public WeightedMomList()
        {
            BufMom = new WeightedMom();
        }

        //Список весов значений
        private readonly List<double> _weights = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            base.AddBufMom(i);
            _weights.Insert(i, ((WeightedMom)BufMom).Weight);
        }
        protected override void AddBufMomEnd()
        {
            base.AddBufMomEnd();
            _weights.Add(((WeightedMom)BufMom).Weight);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            base.SetBufMom(i);
            ((WeightedMom)BufMom).Weight = _weights[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            base.ClearMeans();
            _weights.Clear();
        }
    }
}