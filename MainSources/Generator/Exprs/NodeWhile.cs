using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Цикл, возвращающий значение
    internal class NodeWhile : NodeExpr
    {
        public NodeWhile(ITerminalNode terminal, NodeExpr condition, NodeExpr prog, NodeExpr elseProg)
            : base(terminal)
        {
            _condition = condition;
            _prog = prog;
            _elseProg = elseProg;
        }

        protected override string NodeType { get { return "While"; } }

        //Условие, вычисляеое выражение и значение возвращаемое если цикл ни разу не выполнялся
        private readonly NodeExpr _condition;
        private readonly NodeExpr _prog;
        private readonly NodeExpr _elseProg;

        //Вычисление значения
        public override Mean Process()
        {
            Mean mean = null;
            while (_condition.Process().Boolean)
                mean = _prog.Process();
            return mean ?? _elseProg.Process();
        }
    }

    //----------------------------------------------------------------------------------------------------------
    //Цикл, возвращающий значение
    internal class NodeWhileVoid : NodeVoid
    {
        public NodeWhileVoid(ITerminalNode terminal, NodeExpr condition, NodeVoid prog)
            : base(terminal)
        {
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "While"; } }

        //Условие и выполняемая программа
        private readonly NodeExpr _condition;
        private readonly NodeVoid _prog;

        //Вычисление значения
        public override void Process()
        {
            while (_condition.Process().Boolean)
                _prog.Process();
        }
    }
}