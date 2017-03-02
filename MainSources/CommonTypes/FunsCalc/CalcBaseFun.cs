using System;
using System.Reflection;

namespace CommonTypes
{
    //Константа
    public class ConstBaseFun : CalcBaseFun
    {
        public ConstBaseFun(BaseFunctions funs, string code, int errNum)
            : base(funs, code, errNum) { }

        protected override void CreateDelegateInstance(BaseFunctions funs, MethodInfo met)
        {
            Fun = (ConstDelegate)Delegate.CreateDelegate(typeof(ConstDelegate), funs, met);
        }

        //Делегат для функции без параметров
        public delegate IVal ConstDelegate();
        //Ссылка на реализацию функции
        protected ConstDelegate Fun { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------

    //Одна перегрузка функции, базовый класс для функций разных типов
    public abstract class CalcBaseFun
    {
        protected CalcBaseFun(BaseFunctions funs, //Ссылка на класс реализации функций
                                    string code, //Код реализации функции
                                    int errNum) //Стандартный номер ошибки
        {
            Code = code;
            Functions = funs;
            ErrorNumber = errNum;
            MethodInfo met = typeof(BaseFunctions).GetMethod(code);
            if (met != null) 
                CreateDelegateInstance(funs, met);
        }

        //Создание экземпляра делегата функции
        protected abstract void CreateDelegateInstance(BaseFunctions funs, MethodInfo met);

        //Ссылка на BaseFunctions
        protected BaseFunctions Functions { get; private set; }
        
        //Код функции с буквами типов данных параметров
        public string Code { get; private set; }
        //Стандартный номер ошибки
        public int ErrorNumber { get; private set; }
    }
}