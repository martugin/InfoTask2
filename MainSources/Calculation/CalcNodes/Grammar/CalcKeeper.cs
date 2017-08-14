using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    internal class CalcKeeper : ParsingKeeper
    {
        protected override Node MakeConstNode(ITerminalNode terminal, bool b)
        {
            return new ConstCalcNode(MFactory.NewMean(b));
        }

        protected override Node MakeConstNode(ITerminalNode terminal, int i)
        {
            return new ConstCalcNode(MFactory.NewMean(i));
        }

        protected override Node MakeConstNode(ITerminalNode terminal, double r)
        {
            return new ConstCalcNode(MFactory.NewMean(r));
        }

        protected override Node MakeConstNode(ITerminalNode terminal, DateTime d)
        {
            return new ConstCalcNode(MFactory.NewMean(d));
        }

        protected override Node MakeConstNode(ITerminalNode terminal, string s)
        {
            return new ConstCalcNode(MFactory.NewMean(s));
        }
    }
}