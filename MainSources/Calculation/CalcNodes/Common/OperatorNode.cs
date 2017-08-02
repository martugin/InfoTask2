using System.Collections.Generic;
using System.Linq;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Узел, задающий один оператор
    internal class OperatorNode : Node, ICalcNode
    {
        //Список операций
        private readonly List<ICalcNode> _args = new List<ICalcNode>();
        public List<ICalcNode> Args { get { return _args; } }

        //Возвращаемое значение
        public IVal Value { get { return Args.Last().Value; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Args.Cast<INode>().ToArray());
        }
    }
}