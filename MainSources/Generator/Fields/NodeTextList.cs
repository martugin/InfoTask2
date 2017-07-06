using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Главный узел генерации значения поля
    internal class NodeTextList : ListNode, IExprNode
    {
        public NodeTextList(IEnumerable<Node> children)
            : base(children) { }

        //Проверка формулы
        public DataType Check(ITablStruct tabl)
        {
            var dtype = DataType.Value;
            foreach (var child in Nodes)
                if (child is IExprNode)
                {
                    var res = ((IExprNode)child).Check(tabl);
                    dtype = dtype == DataType.Value ? res : DataType.String;
                } 
                else if (child is IVoidNode)
                    ((IVoidNode)child).Check(tabl);
            return dtype;
        }

        //Генерация значения
        public IReadMean Generate(SubRows row)
        {
            if (Nodes.Length == 1)
                return ((IExprNode) Nodes[0]).Generate(row);
            string s = "";
            foreach (var child in Nodes)
            {
                if (child is IExprNode)
                    s += ((IExprNode)child).Generate(row).String;
                else ((IVoidNode)child).Generate(row);
            }
            return new StringMean(s);
        }
    }
}