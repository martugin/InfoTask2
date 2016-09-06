using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Вычисление значения функции или операции
    internal class NodeFun : NodeKeeper, INodeExpr
    {
        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                       params INodeExpr[] args) //Аргументы
            : base(keeper, terminal)
        {
            _args = args;
        }

        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                       NodeList argsList) //Узел с аргументами
            : base(keeper, terminal)
        {
            _args = argsList.Children.Cast<INodeExpr>().ToArray();
        }

        protected override string NodeType
        {
            get { return "Fun"; }
        }

        //Аргументы
        private readonly INodeExpr[] _args;

        public override string ToTestString()
        {
            return ToTestWithChildren(_args);
        }

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            throw new NotImplementedException();
        }

        //Вычисленное значение
        public Mean Generate(SubRows row)
        {
            throw new NotImplementedException();
        }
    }
}