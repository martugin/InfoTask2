using System;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace CompileLibrary
{
    public class ConstNode : Node
    {
        public ConstNode(Mean mean) : base(null)
        {
            Mean = mean;
        }
        
        public ConstNode(ITerminalNode terminal, bool b) : base(terminal)
        {
            Mean = new BoolMean(b);
        }
        
        public ConstNode(ITerminalNode terminal, int i) : base(terminal)
        {
            Mean = new IntMean(i);
        }

        public ConstNode(ITerminalNode terminal, double r) : base(terminal)
        {
            Mean = new RealMean(r);
        }

        public ConstNode(ITerminalNode terminal, DateTime t) : base(terminal)
        {
            Mean = new TimeMean(t);
        }

        public ConstNode(ITerminalNode terminal, string s) : base(terminal)
        {
            Mean = new StringMean(s);
        }

        public ConstNode(ITerminalNode terminal, DataType dtype, string s) : base(terminal)
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