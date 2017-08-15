using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Узел, задающий параметр
    internal class ParamVal : Val
    {
        public ParamVal(CalcParamInstance param)
        {
            Param = param;
        }

        //Ссылка на экземпляр параметра
        public CalcParamInstance Param { get; private set; }


        public override ICalcVal CalcValue 
        { 
            get { return Param.ParamValue.CalcValue; }
        }

        public override DataType DataType
        {
            get { return CalcValue.DataType; }
        }

        public override object Clone()
        {
            return new ParamVal(Param);
        }
    }
}