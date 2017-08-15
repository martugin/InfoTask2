using System;
using CommonTypes;

namespace Calculation
{
    //Узел - обращение к таблице
    internal class TablNode : CalcNode
    {
        //Таблица

        //Код поля
        public string Code { get; private set; }

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}