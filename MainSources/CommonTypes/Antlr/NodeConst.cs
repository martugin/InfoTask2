using System;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    public class NodeConst : Node
    {
        public NodeConst(Mean mean) : base(null)
        {
            Mean = mean;
        }
        
        public NodeConst(ITerminalNode terminal, bool b) : base(terminal)
        {
            Mean = new MeanBool(b);
        }
        
        public NodeConst(ITerminalNode terminal, int i) : base(terminal)
        {
            Mean = new MeanInt(i);
        }

        public NodeConst(ITerminalNode terminal, double r) : base(terminal)
        {
            Mean = new MeanReal(r);
        }

        public NodeConst(ITerminalNode terminal, DateTime t) : base(terminal)
        {
            Mean = new MeanTime(t);
        }

        public NodeConst(ITerminalNode terminal, string s) : base(terminal)
        {
            Mean = new MeanString(s);
        }

        public NodeConst(ITerminalNode terminal, DataType dtype, string s) : base(terminal)
        {
            Mean = MFactory.NewMean(dtype, s);
        }

        //Тип узла
        protected override string NodeType { get { return Mean.DataType.ToString(); } }

        //Значение константы
        public Mean Mean { get; private set; }
        //Тип данных
        public DataType DataType { get { return Mean.DataType; } }
    }
}