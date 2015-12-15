using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех значений, кроме переменных
    public abstract class Val : IVal
    {
        public Val Value { get { return this; } }
        public abstract ICalcVal CalcValue { get; }
        public abstract DataType DataType { get; }
    }

    //-----------------------------------------------------------------------------------------------

    //Базовый класс для расчетных значений и массивов
    public abstract class CalcVal : Val, ICalcVal
    {
        protected CalcVal(ErrMom err)
        {
            Error = err;
        }
        
        //Общая ошибка на все значение
        public ErrMom Error { get; internal set; }
        public virtual ErrMom TotalError { get { return Error; } }

        public override ICalcVal CalcValue { get { return this; } }
    }
}