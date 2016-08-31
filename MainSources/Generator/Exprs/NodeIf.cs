using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Условие, возвращающее значение
    internal class NodeIf : Node, INodeExpr
    {
        public NodeIf(ITerminalNode terminal, List<INodeExpr> conditions, List<INodeExpr> variants)
            : base(terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "If"; } }

        //Список условий
        private readonly List<INodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<INodeExpr> _variants;

        //Вычисление значения
        public Mean Process(TablRow row)
        {
            for (int i = 0; i < _variants.Count - 1; i++)
                if (_conditions[i].Process(row).Boolean)
                    return _variants[i].Process(row);
            return _variants.Last().Process(row);
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //Условие, возвращающее значение
    internal class NodeIfVoid : Node, INodeVoid
    {
        public NodeIfVoid(ITerminalNode terminal, List<INodeExpr> conditions, List<INodeVoid> variants)
            : base(terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "IfVoid"; } }

        //Список условий
        private readonly List<INodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<INodeVoid> _variants;

        //Вычисление значения
        public void Process(TablRow row)
        {
            for (int i = 0; i < _variants.Count; i++)
                if (i == _conditions.Count || _conditions[i].Process(row).Boolean)
                    _variants[i].Process(row);
        }
    }
}