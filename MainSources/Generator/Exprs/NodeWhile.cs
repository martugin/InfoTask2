using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Цикл, возвращающий значение
    internal class NodeWhile : Node, INodeExpr
    {
        public NodeWhile(ITerminalNode terminal, INodeExpr condition, INodeExpr prog, INodeExpr elseProg)
            : base(terminal)
        {
            _condition = condition;
            _prog = prog;
            _elseProg = elseProg;
        }

        protected override string NodeType { get { return "While"; } }

        //Условие, вычисляеое выражение и значение возвращаемое если цикл ни разу не выполнялся
        private readonly INodeExpr _condition;
        private readonly INodeExpr _prog;
        private readonly INodeExpr _elseProg;

        //Вычисление значения
        public Mean Process(TablRow row)
        {
            Mean mean = null;
            while (_condition.Process(row).Boolean)
                mean = _prog.Process(row);
            return mean ?? _elseProg.Process(row);
        }
    }

    //----------------------------------------------------------------------------------------------------------
    //Цикл, возвращающий значение
    internal class NodeWhileVoid : Node, INodeVoid
    {
        public NodeWhileVoid(ITerminalNode terminal, INodeExpr condition, INodeVoid prog)
            : base(terminal)
        {
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "WhileVoid"; } }

        //Условие и выполняемая программа
        private readonly INodeExpr _condition;
        private readonly INodeVoid _prog;

        //Вычисление значения
        public void Process(TablRow row)
        {
            while (_condition.Process(row).Boolean)
                _prog.Process(row);
        }
    }
}