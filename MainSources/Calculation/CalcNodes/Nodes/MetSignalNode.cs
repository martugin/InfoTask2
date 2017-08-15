using System;
using CommonTypes;

namespace Calculation
{
    //Получение сигнала от объекта
    internal class MetSignalNode : CalcNode
    {
        public MetSignalNode(string code)
        {
            Code = code;
        }

        //Код сигнала
        public string Code { get; private set; }

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}