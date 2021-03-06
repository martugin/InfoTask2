﻿namespace CommonTypes
{
    //Базовый класс для всех значений, кроме переменных
    public abstract class Val : IVal
    {
        public Val Value { get { return this; } }
        public abstract ICalcVal CalcValue { get; }
        public abstract DataType DataType { get; }
        public abstract object Clone();
    }

    //-----------------------------------------------------------------------------------------------

    //Базовый класс для расчетных значений, сегментов и массивов
    public abstract class CalcVal : Val, ICalcVal
    {
        //Общая ошибка на все значение
        public virtual MomErr TotalError { get { return null; } }

        public override ICalcVal CalcValue { get { return this; } }
    }
}