using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Константа
    internal class ConstFunction : FunCalcBase
    {
        public ConstFunction(FunctionsBase funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Делегат для функции без параметров
        public delegate IVal ConstDelegate();
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция
    internal class ScalarFunction : FunScalarBase
    {
        public ScalarFunction(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Скалярные с указанием параметров, используемых в каждой точке
        public delegate void ScalarComplexDelegate(IMom[] par, bool[] cpar);
        //Скалярные с первым не скалярным параметром (Grafic, Tabl)
        public delegate void ScalarObjectDelegate(Val commonVal, IMom[] par);

        //Вычисление скалярной функции
        public IMomList CalcScalar(DataType dataType, ScalarDelegate scalar, params IMeanList[] par)
        {
            return ScalarOrComplexAsList(dataType, scalar, null, null, null, par);
        }

        //Вычисление сложной скалярной функции
        public IMomList CalcScalarComplex(DataType dataType, ScalarComplexDelegate complex, params IMeanList[] par)
        {
            return ScalarOrComplexAsList(dataType, null, complex, null, null, par);
        }

        //Вычисление скалярной функции с нескалярным объектом
        public IMomList CalcScalarObject(DataType dataType, ScalarObjectDelegate objec, Val commonVal, params IMeanList[] par)
        {
            return ScalarOrComplexAsList(dataType, null, null, objec, commonVal, par);
        }

        private IMomList ScalarOrComplexAsList(DataType dataType, ScalarDelegate scalar, ScalarComplexDelegate complex, ScalarObjectDelegate objec, Val commonVal, IMeanList[] par)
        {
            var mpar = new IMom[par.Length];
            var cpar = new bool[par.Length];
            var lists = new List<MList>();
            
            bool isMom = true;
            for (int i = 0; i < par.Length; i++)
            {
                var mom = par[i] as Mom;
                if (mom != null) mpar[i] = mom;
                else
                {
                    isMom = false;
                    var moms = ((MomList)par[i]).Moments;
                    if (moms.Count == 0 && scalar != null)
                        return new MomList(dataType);
                    lists.Add(new MList(moms, i));
                }
            }

            if (isMom)//Одно значение
                return CalcScalarVal(dataType, scalar, complex, objec, commonVal, mpar, cpar); 

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

        //Вычисление одного значения
        private Mom CalcScalarVal(DataType dataType, ScalarDelegate scalar, ScalarComplexDelegate complex, ScalarObjectDelegate objec, Val commonVal, IMom[] mpar, bool[] cpar)
        {
            _scalarRes = new MomEdit(dataType, MaxErr(mpar));
            try
            {
                if (scalar != null) scalar(mpar);
                else if (complex != null) complex(mpar, cpar);
                else objec(commonVal, mpar);    
            }
            catch { PutErr();}
            var m = _scalarRes.ToMom;
            _scalarRes = null;
            return m;
        }
        
    }

    //--------------------------------------------------------------------------------------------------------------------
    //Вспомогательный класс для хранения одного списка значений при вычислении скалярных функций
    internal class MList
    {
        public MList(ReadOnlyCollection<IMom> list, int num)
        {
            _list = list;
            Num = num;
            Pos = -1;
        }

        //Список значений
        private readonly ReadOnlyCollection<IMom> _list;
        //Номер в списке исходных параметров
        public int Num { get; private set; }

        //Текущий обрабатываемый индекс
        public int Pos { get; set; }
        //Время следующего индекса
        public DateTime NextTime
        {
            get
            {
                if (Pos + 1 >= _list.Count)
                    return DateTime.MaxValue;
                return _list[Pos + 1].Time;
            }
        }
}