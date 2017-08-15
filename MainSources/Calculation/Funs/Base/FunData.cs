using CommonTypes;

namespace Calculation
{
    //Данные о значениях параметров функции, сохраненные с предыдущего периода
    public class FunData
    {
        //На входе количество параметров функции
        public FunData(int parsCount)
        {
            ParamsValues = new IReadMean[parsCount];
        }

        //Значения входных параметров функции
        public IReadMean[] ParamsValues { get; private set; }
        //Значение результата функции
        public IReadMean ResultValue { get; set; }
    }
}