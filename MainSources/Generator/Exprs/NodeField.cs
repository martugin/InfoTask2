using System;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Значение поля таблицы
    internal class NodeField : NodeExpr
    {
        public NodeField(ITerminalNode terminal)
            : base(terminal)
        {
            if (terminal != null)
                _field = terminal.Symbol.Text;
        }

        //Имя поля 
        private string _field;

        protected override string NodeType { get { return "Field"; } }

        //Вычисленное значение
        public override Mean Process()
        {
            throw new NotImplementedException();
        }
    }
}