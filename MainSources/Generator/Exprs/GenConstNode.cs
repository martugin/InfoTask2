using System;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел константа для генерации
    internal class GenConstNode : ConstNode, IExprNode 
    {
        public GenConstNode(Mean mean) : base(mean) { }
        public GenConstNode(ITerminalNode terminal, bool b) : base(terminal, b) { }
        public GenConstNode(ITerminalNode terminal, int i) : base(terminal, i) { }
        public GenConstNode(ITerminalNode terminal, double r) : base(terminal, r) { }
        public GenConstNode(ITerminalNode terminal, DateTime t) : base(terminal, t) { }
        public GenConstNode(ITerminalNode terminal, string s) : base(terminal, s) { }
        public GenConstNode(ITerminalNode terminal, DataType dtype, string s) : base(terminal, dtype, s) { }

        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            return DataType;
        }

        //Вычисление значения
        public IMean Generate(SubRows row)
        {
            return Mean;
        }
    }
}