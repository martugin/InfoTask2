using System;
using CommonTypes;

namespace Calculation
{
    //Узел - обращение к переменной
    internal class VarNode : CalcNode
    {
        public VarNode(CalcVar v)
        {
            Var = v;
        }

        //Переменная
        public CalcVar Var { get; private set; }

        //Вычисление значения
        public override IVal Calculate()
        {
            throw new NotImplementedException();
        }
    }
}