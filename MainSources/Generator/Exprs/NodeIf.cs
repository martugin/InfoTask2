using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Условие, возвращающее значение
    internal class NodeIf : NodeExpr
    {
        public NodeIf(ITerminalNode terminal, List<NodeExpr> conditions, List<NodeExpr> variants)
            : base(terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "If"; } }

        //Список условий
        private readonly List<NodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<NodeExpr> _variants;

        //Вычисление значения
        public override Mean Process()
        {
            for (int i = 0; i < _variants.Count - 1; i++)
                if (_conditions[i].Process().Boolean)
                    return _variants[i].Process();
            return _variants.Last().Process();
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //Условие, возвращающее значение
    internal class NodeIfVoid : NodeVoid
    {
        public NodeIfVoid(ITerminalNode terminal, List<NodeExpr> conditions, List<NodeVoid> variants)
            : base(terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "IfVoid"; } }

        //Список условий
        private readonly List<NodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<NodeVoid> _variants;

        //Вычисление значения
        public override void Process()
        {
            for (int i = 0; i < _variants.Count; i++)
                if (i == _conditions.Count || _conditions[i].Process().Boolean)
                    _variants[i].Process();
        }
    }
}