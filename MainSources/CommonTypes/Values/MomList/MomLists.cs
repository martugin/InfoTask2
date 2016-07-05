using System;
using System.Collections.Generic;

namespace CommonTypes
{
    //Список логических значений
    public class MomListBool : MomList
    {
        public MomListBool()
        {
            BufMean = new MeanBool();
        }

        //Список значений
        private readonly List<bool> _means = new List<bool>();

        //Добавить буферное значение в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMean.Boolean);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMean.Boolean);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            BufMean.Boolean = _means[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список целых значений
    public class MomListInt : MomList
    {
        public MomListInt()
        {
            BufMean = new MeanInt();
        }

        //Список значений
        private readonly List<int> _means = new List<int>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMean.Integer);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMean.Integer);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            BufMean.Integer = _means[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных значений
    public class MomListReal : MomList
    {
        public MomListReal()
        {
            BufMean = new MeanReal();
        }

        //Список значений
        private readonly List<double> _means = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMean.Real);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMean.Real);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            BufMean.Real = _means[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список строковых значений
    public class MomListString : MomList
    {
        public MomListString()
        {
            BufMean = new MeanString();
        }

        //Список значений
        private readonly List<string> _means = new List<string>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMean.String);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMean.String);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            BufMean.String = _means[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список значений - дат
    public class MomListTime : MomList
    {
        public MomListTime()
        {
            BufMean = new MeanTime();
        }

        //Список значений
        private readonly List<DateTime> _means = new List<DateTime>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            _means.Insert(i, BufMean.Date);
        }
        protected override void AddBufMomEnd()
        {
            _means.Add(BufMean.Date);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            BufMean.Date = _means[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            _means.Clear();
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных взвешенных значений
    public class MomListWeighted : MomListReal
    {
        public MomListWeighted()
        {
            BufMean = new MomWeighted();
        }

        //Список весов значений
        private readonly List<double> _weights = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            base.AddBufMom(i);
            _weights.Insert(i, ((MomWeighted)BufMean).Weight);
        }
        protected override void AddBufMomEnd()
        {
            base.AddBufMomEnd();
            _weights.Add(((MomWeighted)BufMean).Weight);
        }
        //Загрузить буферное значение из списка 
        public override Mean Mom(int i)
        {
            base.Mom(i);
            ((MomWeighted) BufMean).Weight = _weights[i];
            return BufMean;
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            base.ClearMeans();
            _weights.Clear();
        }
    }
}