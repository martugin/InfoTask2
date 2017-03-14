using System;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    internal partial class CalcFunctions : BaseFunctions
    {
        //Условие для фильтрации списка функций
        protected override string FunsWhereCondition
        {
            get { return "Functions.IsCalc = True"; }
        }

        //Создание функций разных типов
        protected override CalcBaseFun CreateFun(string code, string ftype, int errNum)
        {
            switch (ftype)
            {
                case "Scalar":
                    return new ScalarFun(this, code, errNum);
                case "Const":
                    return new ConstFun(this, code, errNum);
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
        public DateTime PeriodBegin
        {
            get { throw new NotImplementedException();}
        }
        public DateTime PeriodEnd
        {
            get { throw new NotImplementedException(); }
        }

        //Вычисление скалярной функции для списков мгновенных значений
        internal IMean CalcScalar(DataType dataType, //Возвращаемый тип данных
                                                IMean[] par, //Список аргументов
                                                bool isComplex, //При вычислении используются флаги значений аргументов
                                                Action<IMean[], bool[]> action) //Действие, выполняющее вычисление для одного момента времени
        {
            SetScalarDataType(dataType);
            var mpar = new IMean[par.Length];
            var cpar = new bool[par.Length];
            
            bool isMom = !isComplex;
            for (int i = 0; i < par.Length; i++)
            {
                var mean = par[i] as Mean;
                if (mean != null) mpar[i] = mean;
                else
                {
                    isMom = false;
                    if (par[i].Count == 0 && !isComplex) 
                        return MFactory.NewList(dataType);
                }
            }

            if (isMom) //Одно значение
            {
                DateTime t = Static.MinDate;
                foreach (var mean in mpar)
                    if (mean.Time > t)
                        t = mean.Time;
                if (t == Static.MinDate) t = PeriodBegin;
                CalcScalarFun(mpar, () => action(mpar, cpar));
                return ScalarRes.ToMom(t);
            }
            
            //Список значений
            var rlist = MFactory.NewList(dataType);
            while (true)
            {
                DateTime ctime = Static.MaxDate;
                foreach (var list in par)
                    if (list.NextTime < ctime) ctime = list.NextTime;
                if (ctime == Static.MaxDate) break;

                for (int i = 0; i < par.Length; i++)
                {
                    var list = par[i];
                    cpar[i] = list.NextTime == ctime;
                    if (cpar[i]) list.CurNum++;
                    mpar[i] = list.ToMean();
                }
                CalcScalarFun(mpar, () => action(mpar, cpar));
                rlist.AddMom(ScalarRes.ToMom(ctime));
            }
            return rlist;
        }
    }
}