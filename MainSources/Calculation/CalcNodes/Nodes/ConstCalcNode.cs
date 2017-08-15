using System;
using CommonTypes;

namespace Calculation
{
    //Узел - константа
    internal class ConstCalcNode : CalcNode 
    {
        public ConstCalcNode(Mean mean)
        {
            _mean = mean;
        }

        //Значение
        private readonly Mean _mean;
        
        //Присвоение
        public override IVal Calculate()
        {
            throw new NotImplementedException();
            return _mean;
        }
    }
}