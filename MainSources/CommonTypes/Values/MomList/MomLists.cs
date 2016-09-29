using System;
using System.Collections.Generic;

namespace CommonTypes
{
    //Список логических значений
    public class MomListBool : MomList
    {
        public MomListBool()
        {
            BufMom = new MeanBool();
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
    public class MomListInt : MomList
    {
        public MomListInt()
        {
            BufMom = new MeanInt();
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
    public class MomListReal : MomList
    {
        public MomListReal()
        {
            BufMom = new MeanReal();
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
    public class MomListString : MomList
    {
        public MomListString()
        {
            BufMom = new MeanString();
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
    public class MomListTime : MomList
    {
        public MomListTime()
        {
            BufMom = new MeanTime();
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
    public class MomListWeighted : MomListReal
    {
        public MomListWeighted()
        {
            BufMom = new MomWeighted();
        }

        //Список весов значений
        private readonly List<double> _weights = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddBufMom(int i)
        {
            base.AddBufMom(i);
            _weights.Insert(i, ((MomWeighted)BufMom).Weight);
        }
        protected override void AddBufMomEnd()
        {
            base.AddBufMomEnd();
            _weights.Add(((MomWeighted)BufMom).Weight);
        }
        //Загрузить буферное значение из списка 
        protected override void SetBufMom(int i)
        {
            base.SetBufMom(i);
            ((MomWeighted)BufMom).Weight = _weights[i];
        }

        //Очистка значений
        protected override void ClearMeans()
        {
            base.ClearMeans();
            _weights.Clear();
        }
    }
}