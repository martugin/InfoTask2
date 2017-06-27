using System;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Константа в расчетном выражении
    internal class TablikConstNode : ConstNode, ISyntacticNode, IExprNode
    {
        public TablikConstNode(Mean mean) : base(mean) { }
        public TablikConstNode(ITerminalNode terminal, bool b) : base(terminal, b) { }
        public TablikConstNode(ITerminalNode terminal, int i) : base(terminal, i) { }
        public TablikConstNode(ITerminalNode terminal, double r) : base(terminal, r) { }
        public TablikConstNode(ITerminalNode terminal, DateTime t) : base(terminal, t) { }
        public TablikConstNode(ITerminalNode terminal, string s) : base(terminal, s) { }
        public TablikConstNode(ITerminalNode terminal, DataType dtype, string s) : base(terminal, dtype, s) { }

        public IExprNode DefineSemantic()
        {
            return this;
        }

        //Тип данных
        public ITablikType Type { get; private set; }

        //Определение расчетного типа данных
        public ITablikType DefineType()
        {
            return Type = new SimpleType(DataType);
        }

        //Запись в скомпилированое выражение
        public string CompiledText()
        {
            string val = Mean.DataType != DataType.String ? Mean.String : "\'" + Mean.String + "\'";
            return DataType.ToEnglish() + "!" + val;
        }
    }
}