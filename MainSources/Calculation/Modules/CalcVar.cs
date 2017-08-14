using CommonTypes;

namespace Calculation
{
    //Переменная
    internal class CalcVar
    {
        public CalcVar(string code)
        {
            Code = code;
        }

        //Код
        public string Code { get; private set; }
        //Значение
        public IVal Value { get; set; }
    }
}