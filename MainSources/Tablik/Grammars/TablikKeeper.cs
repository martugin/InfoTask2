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

        //Расчетный параметр
        internal TablikParam Param { get; private set; }

        protected override Node MakeConstNode(ITerminalNode terminal, bool b)
        {
            return new ConstNode(terminal, b);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, int i)
        {
            return new ConstNode(terminal, i);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, double r)
        {
            return new ConstNode(terminal, r);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, DateTime d)
        {
            return new ConstNode(terminal, d);
        }

        protected override Node MakeConstNode(ITerminalNode terminal, string s)
        {
            return new ConstNode(terminal, s);
        }
    }
}