using System;
using System.Linq;
using System.Reflection;
using CommonTypes;

namespace Calculation
{
    //Интерфейс для всех функций расчета
    internal interface IFunction
    {
        //Вычислить значение
        IVal Calculate(DataType resultType, IVal[] par);
    }

    //---------------------------------------------------------------------------------------------------
    //Константа
    internal class ConstFunction : FunCalcBase, IFunction
    {
        public ConstFunction(FunctionsCalc funs, string code, int errNum)
            : base(funs, code, errNum) { }

        protected override void CreateDelegateInstance(FunctionsBase funs, MethodInfo met)
        {
            _fun = (ConstDelegate)Delegate.CreateDelegate(typeof(ConstDelegate), funs, met);
        }

        //Делегат для функции без параметров
        public delegate IVal ConstDelegate();
        //Ссылка на реализацию функции
        private ConstDelegate _fun;

        public IVal Calculate(DataType resultType, IVal[] par)
        {
            return _fun();
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчета
    internal class ScalarFunction : FunScalarBase, IFunction
    {
        public ScalarFunction(FunctionsCalc funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Вычислить значение
        public IVal Calculate(DataType resultType, IVal[] par)
        {
            var fs = (FunctionsCalc)Functions;
            return fs.CalcScalarList(resultType, 
                                                par.Cast<IMeanList>().ToArray(), false,
                                                (moms, flags) => Fun(moms));
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчетас указанием параметров, используемых в каждой точке
    internal class ScalarComplexFunction : FunCalcBase, IFunction
    {
        public ScalarComplexFunction(FunctionsCalc funs, string code, int errNum)
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
        public IVal Calculate(DataType resultType, IVal[] par)
        {
            var fs = (FunctionsCalc)Functions;
            return fs.CalcScalarList(resultType,
                                                par.Cast<IMeanList>().ToArray(), true, 
                                                (moms, flags) => _fun(moms, flags));
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Скалярная функция расчетас указанием параметров, используемых в каждой точке
    internal class ScalarObjectFunction : FunCalcBase, IFunction
    {
        public ScalarObjectFunction(FunctionsCalc funs, string code, int errNum)
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
        public IVal Calculate(DataType resultType, IVal[] par)
        {
            var fs = (FunctionsCalc)Functions;
            var args = new IMeanList[par.Length-1];
            for (int i = 1; i < par.Length; i++)
                args[i] = (IMeanList) par[i];
            return fs.CalcScalarList(resultType, args, false, (moms, flags) => _fun(par[0], moms));
        }
    }
}