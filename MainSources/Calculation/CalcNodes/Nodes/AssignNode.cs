using System;
using CommonTypes;

namespace Calculation
{
    //Узел - присвоение значения переменной
    internal class AssignNode : CalcNode
    {
        public AssignNode(CalcVar v)
        {
            Var = v;
        }

        //Переменная
        public CalcVar Var { get; private set; }

        //Присвоение
        public override IVal Calculate()
        {
            throw new NotImplementedException();
            return new VoidVal();
        }
    }
}