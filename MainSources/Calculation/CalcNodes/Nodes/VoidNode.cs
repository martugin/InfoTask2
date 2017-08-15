using CommonTypes;

namespace Calculation
{
    internal class VoidNode : CalcNode
    {
        private readonly VoidVal _value = new VoidVal();

        //Вычисление значения
        public override IVal Calculate()
        {
            return _value;
        }
    }
}