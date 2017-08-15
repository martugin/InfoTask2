using System;
using CommonTypes;

namespace Calculation
{
    internal class MetNode : CalcNode
    {
        public MetNode(string code)
        {
            Code = code;
        }

        //Код метода
        public string Code { get; private set; }

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}