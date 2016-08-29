using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Вычисление значения функции или операции
    internal class NodeFun : NodeExpr
    {
        public NodeFun(ITerminalNode terminal, //Имя функции
                                params NodeExpr[] args) //Аргументы
            : base(terminal)
        {
            if (terminal != null)
                _args = args;
        }

        public NodeFun(ITerminalNode terminal, //Имя функции
                                NodeList argsList) //Узел с аргументами
            : base(terminal)
        {
            if (terminal != null)
                _args = argsList.Children.Select(a => (NodeExpr)a).ToArray();
        }

        protected override string NodeType { get { return "Fun"; } }

        //Аргументы
        private readonly NodeExpr[] _args;

        public override string ToTestString()
        {
            return ToTestWithChildren(_args);
        }

        //Вычисленное значение
        public override Mean GetMean()
        {
            throw new NotImplementedException();
        }
    }
}