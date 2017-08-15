using System;
using CommonTypes;

namespace Calculation
{
    //Узел, задающий параметр
    internal class ParamNode : CalcNode
    {
        public ParamNode(CalcParamInstance par)
        {
            _paramVal = new ParamVal(par);
        }
        
        //Значение
        private readonly ParamVal _paramVal;

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}