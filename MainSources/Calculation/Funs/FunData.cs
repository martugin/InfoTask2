using CommonTypes;

namespace Calculation
{
    //Данные о значениях параметров функции, сохраненные с предыдущего периода
    public class FunData
    {
        //На входе количество параметров функции
        public FunData(int parsCount)
        {
            ParamsValues = new IMean[parsCount];
        }

        //Значения входных параметров функции
        public IMean[] ParamsValues { get; private set; }
        //Значение результата функции
        public IMean ResultValue { get; set; }
    }
}