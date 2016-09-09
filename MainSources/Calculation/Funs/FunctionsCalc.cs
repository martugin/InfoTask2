using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    internal partial class FunctionsCalc : FunctionsBase
    {
        //Условие для фильтрации списка функций
        protected override string FunsWhereCondition
        {
            get { return "Functions.NotLoadCalc = False"; }
        }

        //Создание функций разных типов
        protected override FunCalcBase CreateFun(string code, string ftype, int errNum)
        {
            switch (ftype)
            {
                case "Scalar":
                    return new ScalarFunction(this, code, errNum);
                case "Const":
                    return new ConstFunction(this, code, errNum);
            }
            return null;
        }

        //Код модуля
        protected override string ErrSourceCode
        {
            get { throw new NotImplementedException(); }
        }

        //Текущий порожденный расчетный параметр
        protected override IContextable Contextable
        {
            get { throw new NotImplementedException(); }
        }

        //Начало и конец периода расчета
        public DateTime BeginPeriod
        {
            get { throw new NotImplementedException();}
        }
        public DateTime EndPeriod
        {
            get { throw new NotImplementedException(); }
        }

        //Вычисление скалярной функции для списков мгновенных значений
        internal IMomListRead CalcScalar(DataType dataType, //Возвращаемый тип данных
                                                            IMeanList[] par, //Список аргументов
                                                            Action<IMean[]> action) //Действие, выполняющее вычисление для одного момента времени
        {
            SetScalarDataType(dataType);
            var mpar = new IMean[par.Length];
            var lists = new List<ScalarPar>();

            bool isMom = true;
            for (int i = 0; i < par.Length; i++)
            {
                var mean = par[i] as IMean;
                if (mean != null) mpar[i] = mean;
                else
                {
                    isMom = false;
                    var moms = (IMomListRead)par[i];
                    if (moms.Count == 0) return MFactory.NewList(dataType);
                    lists.Add(new ScalarPar(moms, i));
                }
            }

            if (isMom) //Одно значение
            {
                DateTime t = Different.MinDate;
                foreach (var mean in mpar)
                    if (mean is IMom && ((IMom)mean).Time > t)
                        t = ((IMom)mean).Time;
                if (t == Different.MinDate) t = BeginPeriod;
                CalcScalarFun(mpar, () => action(mpar));
                return ScalarRes.CloneMom(t);
            }


            //Список значений
            var rlist = MFactory.NewList(dataType);
            while (true)
            {
                DateTime ctime = Different.MaxDate;
                foreach (var list in lists)
                    if (list.NextTime < ctime) ctime = list.NextTime;
                if (ctime == Different.MaxDate) break;

                foreach (var list in lists)
                {
                    if (list.NextTime == ctime)
                        list.Pos++;
                    mpar[list.Num] = ((IMomListRead)par[list.Num]). (Interpolation, list.Pos, ctime);
                }
                rlist.AddMom(CalcScalarVal(dataType, scalar, complex, objec, commonVal, mpar, cpar));
            }
            return rlist;
        }

        //Вычисление скалярной функции для списков мгновенных значений
        internal IMomList CalcScalarList(DataType dataType, //Возвращаемый тип данных
                                                         IMeanList[] par, //Список аргументов
                                                         bool isComplex, //При вычислении используются флаги значений аргументов
                                                         Action<IMean[], bool[]> action) //Действие, выполняющее вычисление для одного момента времени
        {
            SetScalarDataType(dataType);
            var mpar = new IMean[par.Length];
            var cpar = new bool[par.Length];
            var lists = new List<ScalarPar>();

            bool isMom = true;
            for (int i = 0; i < par.Length; i++)
            {
                var mean = par[i] as IMean;
                if (mean != null) mpar[i] = mean;
                else
                {
                    isMom = false;
                    var moms = (IMomListRead)par[i];
                    if (moms.Count == 0 && !isComplex)
                        return MFactory.NewList(dataType);
                    lists.Add(new ScalarPar(moms, i));
                }
            }

            if (isMom) //Одно значение
            {
                CalcScalarFun(mpar, () => action(mpar, cpar));
            }
                

            //Список значений
            var rlist = new MomList(dataType, MaxErr(par));
            while (true)
            {
                for (int i = 0; i < cpar.Length; i++)
                    cpar[i] = false;
                DateTime ctime = Different.MaxDate;
                foreach (var list in lists)
                    if (list.NextTime < ctime) ctime = list.NextTime;
                if (ctime == Different.MaxDate) break;

                foreach (var list in lists)
                {
                    if (list.NextTime == ctime)
                    {
                        list.Pos++;
                        cpar[list.Num] = true;
                    }
                    mpar[list.Num] = ((MomList)par[list.Num]).Interpolation(Interpolation, list.Pos, ctime);
                }
                rlist.AddMom(CalcScalarVal(dataType, scalar, complex, objec, commonVal, mpar, cpar));
            }
            return rlist;
        }
    }
}