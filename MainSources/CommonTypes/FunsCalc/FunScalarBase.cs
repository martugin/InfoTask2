using System;
using System.Reflection;

namespace CommonTypes
{
    //Базовый класс для скалярных функций
    public abstract class FunScalarBase : FunCalcBase
    {
        protected FunScalarBase(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum)
        {
            MethodInfo met = typeof(FunctionsBase).GetMethod(code);
            if (met != null) 
                _fun = (ScalarDelegate)Delegate.CreateDelegate(typeof(ScalarDelegate), funs, met);
        }

        //Делегат скалярных функций
        private delegate void ScalarDelegate(IMean[] par);

        //Ссылка на реализацию функции
        private readonly ScalarDelegate _fun;
        //Значение для промежуточного результата вычисления
        protected MomEdit ScalarRes { get; set; }

        //Промежуточное вычисление 
        protected void CalcScalar(IMean[] par)
        {
            try
            {
                ScalarRes.MakeDefaultValue();
                ScalarRes.Error = null;
                foreach (var mean in par)
                    ScalarRes.AddError(mean.Error);
                _fun(par);
                if (double.IsNaN(ScalarRes.Real))
                    Functions.PutErr();
            }
            catch { Functions.PutErr();}
        }
    }
}