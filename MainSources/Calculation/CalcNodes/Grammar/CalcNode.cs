using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    internal abstract class CalcNode : Node, ICalcNode
    {
        //Значения
        public abstract IVal Value { get; }
        //Аргументы
        private readonly List<ICalcNode> _args = new List<ICalcNode>();
        public List<ICalcNode> Args { get { return _args; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Args.ToArray());
        }
    }
}