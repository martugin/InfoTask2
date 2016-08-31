using System;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел константа для генерации
    internal class NodeGenConst : NodeConst, INodeExpr 
    {
        public NodeGenConst(Mean mean) : base(mean) { }
        public NodeGenConst(ITerminalNode terminal, bool b) : base(terminal, b) { }
        public NodeGenConst(ITerminalNode terminal, int i) : base(terminal, i) { }
        public NodeGenConst(ITerminalNode terminal, double r) : base(terminal, r) { }
        public NodeGenConst(ITerminalNode terminal, DateTime t) : base(terminal, t) { }
        public NodeGenConst(ITerminalNode terminal, string s) : base(terminal, s) { }
        public NodeGenConst(ITerminalNode terminal, DataType dtype, string s) : base(terminal, dtype, s) { }

        //Вычисление значения
        public Mean Process(TablRow row)
        {
            return Mean;
        }
    }
}