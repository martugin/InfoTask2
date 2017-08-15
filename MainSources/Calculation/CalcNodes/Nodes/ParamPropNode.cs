using System;
using CommonTypes;

namespace Calculation
{
    //Узел - получение свойства параметра
    internal class ParamPropNode : CalcNode
    {
        public ParamPropNode(string propCode)
        {
            PropCode = propCode;
        }

        //Код свойства
        public string PropCode { get; private set; }

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}