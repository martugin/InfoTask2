using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один экземпляр расчетного параметра
    internal class CalcParamInstance
    {
        public CalcParamInstance(CalcParam param, List<IVal> inputs)
        {
            Param = param;
        }

        //Ссылка на расчетный параметр
        public CalcParam Param { get; private set; }
        //Значение 
        public IVal ParamValue { get; set; }
        //Словарь переменных
        private readonly DicS<CalcVar> _vars = new DicS<CalcVar>();
        public DicS<CalcVar> Vars { get { return _vars; } }
    }
}