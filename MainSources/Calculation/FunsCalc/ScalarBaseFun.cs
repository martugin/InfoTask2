using System;
using System.Reflection;
using CommonTypes;

namespace Calculation
{
    //Базовый класс для скалярных функций
    public abstract class ScalarBaseFun : CalcBaseFun
    {
        protected ScalarBaseFun(BaseFunctions funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Делегат скалярных функций
        public delegate void ScalarDelegate(IReadMean[] par);
        //Ссылка на реализацию функции
        public ScalarDelegate Fun { get; private set; }
        //Создание экземпляра делегата функции
        protected override void CreateDelegateInstance(BaseFunctions funs, MethodInfo met)
        {
            Fun = (ScalarDelegate) Delegate.CreateDelegate(typeof (ScalarDelegate), funs, met);
        }
    }
}