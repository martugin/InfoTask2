using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonTypes;

namespace Calculation
{
    //Функция работы со списками мгновенных значений
    internal class MomentsFun : CalcBaseFun, IFunction 
    {
        public MomentsFun(BaseFunctions funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        protected override void CreateDelegateInstance(BaseFunctions funs, MethodInfo met)
        {
            _fun = (MomentsDelegate)Delegate.CreateDelegate(typeof(MomentsDelegate), funs, met);
        }

        //Делегат функции
        public delegate IReadMean MomentsDelegate(FunData data, DataType dataType, params IReadMean[] par);
        //Ссылка на реализацию функции
        private MomentsDelegate _fun;

        //Вычислить значение
        public IVal Calculate(DataType resultType, IEnumerable<IVal> par, FunData funData)
        {
            return _fun(funData, resultType, par.Cast<IReadMean>().ToArray());
        }
    }
}