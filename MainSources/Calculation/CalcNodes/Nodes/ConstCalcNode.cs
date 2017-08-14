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
        public override IVal Value { get { return _mean; } }
    }
}