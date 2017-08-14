using System;
using System.Text;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Константа в расчетном выражении
    internal class TablikConstNode : ConstNode, IExprNode
    {
        public TablikConstNode(Mean mean) : base(mean) { }
        public TablikConstNode(ITerminalNode terminal, bool b) : base(terminal, b) { }
        public TablikConstNode(ITerminalNode terminal, int i) : base(terminal, i) { }
        public TablikConstNode(ITerminalNode terminal, double r) : base(terminal, r) { }
        public TablikConstNode(ITerminalNode terminal, DateTime t) : base(terminal, t) { }
        public TablikConstNode(ITerminalNode terminal, string s) : base(terminal, s) { }
        public TablikConstNode(ITerminalNode terminal, DataType dtype, string s) : base(terminal, dtype, s) { }

        //Тип данных
        public ITablikType Type { get; private set; }

        //Определение расчетного типа данных
        public void DefineType()
        {
            Type = new SimpleType(DataType);
        }

        //Записать текст в скомпилированное выражение
        public void SaveCompiled(StringBuilder sb)
        {
            string val = Mean.DataType != DataType.String ? Mean.String : "\'" + Mean.String + "\'";
            sb.Append("Const").Append(DataType.ToEnglish()).Append("!").Append(val).Append("; ");
        }
    }
}