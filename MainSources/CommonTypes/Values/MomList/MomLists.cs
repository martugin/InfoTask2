using System;
using System.Collections.Generic;

namespace CommonTypes
{
    //Список логических значений
    public class MomListBool : MomList
    {
        //Список значений
        private readonly List<bool> _means = new List<bool>();

        //Текущее значение
        private readonly Mean _curMean = new MeanBool();
        protected override Mean CurMean { get { return _curMean; } }
        
        protected override void GetCurMom(int i)
        {
            CurMean.Boolean = _means[i];
        }
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Boolean);
        }
        protected override void AddCurMom()
        {
            _means.Add(CurMean.Boolean);
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список целых значений
    public class MomListInt : MomList
    {
        //Список значений
        private readonly List<int> _means = new List<int>();

        //Текущее значение
        private readonly Mean _curMean = new MeanInt();
        protected override Mean CurMean { get { return _curMean; } }

        protected override void GetCurMom(int i)
        {
            CurMean.Integer = _means[i];
        }
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Integer);
        }
        protected override void AddCurMom()
        {
            _means.Add(CurMean.Integer);
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных значений
    public class MomListReal : MomList
    {
        //Список значений
        private readonly List<double> _means = new List<double>();

        //Текущее значение
        private readonly Mean _curMean = new MeanReal();
        protected override Mean CurMean { get { return _curMean; } }

        protected override void GetCurMom(int i)
        {
            CurMean.Real = _means[i];
        }
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Real);
        }
        protected override void AddCurMom()
        {
            _means.Add(CurMean.Real);
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список строковых значений
    public class MomListString : MomList
    {
        //Список значений
        private readonly List<string> _means = new List<string>();

        //Текущее значение
        private readonly Mean _curMean = new MeanString();
        protected override Mean CurMean { get { return _curMean; } }

        protected override void GetCurMom(int i)
        {
            CurMean.String = _means[i];
        }
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.String);
        }
        protected override void AddCurMom()
        {
            _means.Add(CurMean.String);
        }
    }

    //--------------------------------------------------------------------------------------------
    //Список действительных значений
    public class MomListTime : MomList
    {
        //Список значений
        private readonly List<DateTime> _means = new List<DateTime>();

        //Текущее значение
        private readonly Mean _curMean = new MeanTime();
        protected override Mean CurMean { get { return _curMean; } }

        protected override void GetCurMom(int i)
        {
            CurMean.Date= _means[i];
        }
        protected override void AddCurMom(int i)
        {
            _means.Insert(i, CurMean.Date);
        }
        protected override void AddCurMom()
        {
            _means.Add(CurMean.Date);
        }
    }
}