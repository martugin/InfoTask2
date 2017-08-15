using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Узел, задающий один оператор
    internal class OperatorNode : Node, ICalcNode
    {
        //Список операций
        private readonly List<ICalcNode> _argsList = new List<ICalcNode>();
        public List<ICalcNode> ArgsList { get { return _argsList; } }
        public IEnumerable<ICalcNode> Args { get { return ArgsList; } }

        //Вычисление значений
        public IVal Calculate()
        {
            IVal val = null;
            foreach (var arg in ArgsList)
                val = arg.Calculate();
            return val;
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(ArgsList.ToArray());
        }
    }
}