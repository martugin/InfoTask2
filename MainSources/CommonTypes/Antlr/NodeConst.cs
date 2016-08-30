using System;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    public class NodeConst : NodeExpr
    {
        public NodeConst(ITerminalNode terminal, bool b) : base(terminal)
        {
            _mean = new MeanBool(b);
        }

        public NodeConst(ITerminalNode terminal, int i) : base(terminal)
        {
            _mean = new MeanInt(i);
        }

        public NodeConst(ITerminalNode terminal, double r) : base(terminal)
        {
            _mean = new MeanReal(r);
        }

        public NodeConst(ITerminalNode terminal, DateTime t) : base(terminal)
        {
            _mean = new MeanTime(t);
        }

        public NodeConst(ITerminalNode terminal, string s) : base(terminal)
        {
            _mean = new MeanString(s);
        }

        public NodeConst(ITerminalNode terminal, DataType dtype, string s) : base(terminal)
        {
            _mean = MFactory.NewMean(dtype, s);
        }

        //Тип узла
        protected override string NodeType { get { return _mean.DataType.ToString(); } }

        //Значение константы
        private readonly Mean _mean;
        public override Mean Process()
        {
            return _mean;
        }
    }
}