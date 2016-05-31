using System;
using System.Collections.Generic;

namespace CommonTypes
{
    //Список логических значений
    public class MomListBool : MomList
    {
        public MomListBool()
        {
            CurMean = new MeanBool();
        }

        //Список значений
        private readonly List<bool> _means = new List<bool>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Boolean);
        }
        protected override void AddCurMomEnd()
        {
            _means.Add(CurMean.Boolean);
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
            CurMean = new MeanInt();
        }

        //Список значений
        private readonly List<int> _means = new List<int>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Integer);
        }
        protected override void AddCurMomEnd()
        {
            _means.Add(CurMean.Integer);
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
            CurMean = new MeanReal();
        }

        //Список значений
        private readonly List<double> _means = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Real);
        }
        protected override void AddCurMomEnd()
        {
            _means.Add(CurMean.Real);
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
            CurMean = new MeanString();
        }

        //Список значений
        private readonly List<string> _means = new List<string>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.String);
        }
        protected override void AddCurMomEnd()
        {
            _means.Add(CurMean.String);
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
            CurMean = new MeanTime();
        }

        //Список значений
        private readonly List<DateTime> _means = new List<DateTime>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Date);
        }
        protected override void AddCurMomEnd()
        {
            _means.Add(CurMean.Date);
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
            CurMean = new MomWeighted();
        }

        //Список весов значений
        private readonly List<double> _weights = new List<double>();

        //Добавить текущее значение для добавления в список по индексу или в конец
        protected override void AddCurMom(int i)
        {
            base.AddCurMom(i);
            _weights.Insert(i, ((MomWeighted)CurMean).Weight);
        }
        protected override void AddCurMomEnd()
        {
            base.AddCurMomEnd();
            _weights.Add(((MomWeighted)CurMean).Weight);
        }
        //Очистка значений
        protected override void ClearMeans()
        {
            base.ClearMeans();
            _weights.Clear();
        }
    }
}