using CommonTypes;

namespace Calculation
{
    //Пустое значение
    internal class VoidVal : Val
    {
        public override ICalcVal CalcValue { get { return null;  }}

        public override DataType DataType
        {
            get { return DataType.Void;}
        }

        public override object Clone()
        {
            return new VoidVal();
        }
    }
}