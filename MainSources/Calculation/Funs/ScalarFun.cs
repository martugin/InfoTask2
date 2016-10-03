using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonTypes;

namespace Calculation
{
    //Интерфейс для всех функций расчета
    internal interface IFunction
    {
        //Вычислить значение
        IVal Calculate(DataType resultType, //Тип данных результата
                             IEnumerable<IVal> par, //Входные параметры
                             FunData funData); //Значения с предыдущего периода
    }

    //---------------------------------------------------------------------------------------------------

    //Константа
    internal class ConstFun : ConstBaseFun, IFunction
    {
        public ConstFun(FunctionsCalc funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Вычислить значение
        public IVal Calculate(DataType resultType, IEnumerable<IVal> par, FunData funData)
        {
            return Fun();
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчета
    internal class ScalarFun : ScalarBaseFun, IFunction
    {
        public ScalarFun(FunctionsCalc funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Вычислить значение
        public IVal Calculate(DataType resultType, IEnumerable<IVal> par, FunData funData)
        {
            var fs = (FunctionsCalc)Functions;
            return fs.CalcScalar(resultType, par.Cast<IMean>().ToArray(), false,
                                           (moms, flags) => Fun(moms));
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчетас указанием параметров, используемых в каждой точке
    internal class ScalarComplexFun : CalcBaseFun, IFunction
    {
        public ScalarComplexFun(FunctionsCalc funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Делегат и экземпляр
        public delegate void ScalarComplexDelegate(IMean[] par, bool[] cpar);
        private ScalarComplexDelegate _fun;
        //Создание экземпляра делегата функции
        protected override void CreateDelegateInstance(FunctionsBase funs, MethodInfo met)
        {
            _fun = (ScalarComplexDelegate)Delegate.CreateDelegate(typeof(ScalarComplexDelegate), funs, met);
        }
        
        //Вычислить значение
        public IVal Calculate(DataType resultType, IEnumerable<IVal> par, FunData funData)
        {
            var fs = (FunctionsCalc)Functions;
            return fs.CalcScalar(resultType, par.Cast<IMean>().ToArray(), true, 
                                           (moms, flags) => _fun(moms, flags));
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчетас указанием параметров, используемых в каждой точке
    internal class ScalarObjectFun : CalcBaseFun, IFunction
    {
        public ScalarObjectFun(FunctionsCalc funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Делегат и экземпляр
        public delegate void ScalarObjectDelegate(IVal commonVal, IMean[] par);
        private ScalarObjectDelegate _fun;
        //Создание экземпляра делегата функции
        protected override void CreateDelegateInstance(FunctionsBase funs, MethodInfo met)
        {
            _fun = (ScalarObjectDelegate)Delegate.CreateDelegate(typeof(ScalarObjectDelegate), funs, met);
        }

        //Вычислить значение
        public IVal Calculate(DataType resultType, IEnumerable<IVal> par, FunData funData)
        {
            var arr = par.ToArray();
            var fs = (FunctionsCalc)Functions;
            var args = new IMean[arr.Length-1];
            for (int i = 1; i < arr.Length; i++)
                args[i] = (IMean) arr[i];
            return fs.CalcScalar(resultType, args, false, (moms, flags) => _fun(arr[0], moms));
        }
    }
}