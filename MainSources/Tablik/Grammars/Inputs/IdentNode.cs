using System;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Узел - идентификатор
    internal class IdentNode : Node
    {
        public IdentNode(ITerminalNode terminal) : base(terminal)
        {
            Text = terminal.Symbol.Text;
        }

        //Текст идентификатора
        public string Text { get; private set; }

        //Тип узла
        protected override string NodeType { get { return "Ident"; } }
    }
}