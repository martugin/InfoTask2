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
        }

        //Ссылка на FunctionsBase
        protected FunctionsBase Functions { get; private set; }
        
        //Код функции с буквами типов данных параметров
        public string Code { get; private set; }
        //Стандартный номер ошибки
        public int ErrorNumber { get; private set; }
    }
}