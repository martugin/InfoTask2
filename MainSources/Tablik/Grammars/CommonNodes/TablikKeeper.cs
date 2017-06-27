using System;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Накопление ошибок параметра в Tablik
    internal class TablikKeeper : ParsingKeeper
    {
        public TablikKeeper(TablikParam param)
        {
            Param = param;
        }

        //Текущий расчетный параметр
        internal TablikParam Param { get; private set; }
        
        protected override Node MakeConstNode(ITerminalNode terminal, bool b)
        {
            return new TablikConstNode(terminal, b);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, int i)
        {
            return new TablikConstNode(terminal, i);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, double r)
        {
            return new TablikConstNode(terminal, r);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, DateTime d)
        {
            return new TablikConstNode(terminal, d);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, string s)
        {
            return new TablikConstNode(terminal, s);
        }
    }
}