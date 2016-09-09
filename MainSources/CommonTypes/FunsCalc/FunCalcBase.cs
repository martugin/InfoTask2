using System.Reflection;

namespace CommonTypes
{
    //Одна перегрузка функции, базовый класс для функций разных типов
    public abstract class FunCalcBase
    {
        protected FunCalcBase(FunctionsBase funs, //Ссылка на класс реализации функций
                                    string code, //Код реализации функции
                                    int errNum) //Стандартный номер ошибки
        {
            Code = code;
            Functions = funs;
            ErrorNumber = errNum;
            MethodInfo met = typeof(FunctionsBase).GetMethod(code);
            if (met != null) 
                CreateDelegateInstance(funs, met);
        }

        //Создание экземпляра делегата функции
        protected abstract void CreateDelegateInstance(FunctionsBase funs, MethodInfo met);

        //Ссылка на FunctionsBase
        protected FunctionsBase Functions { get; private set; }
        
        //Код функции с буквами типов данных параметров
        public string Code { get; private set; }
        //Стандартный номер ошибки
        public int ErrorNumber { get; private set; }
    }
}