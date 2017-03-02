using System.Collections.Generic;
using CommonTypes;

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
            foreach (var child in Children)
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
        public IMean Generate(SubRows row)
        {
            if (Children.Count == 1)
                return ((IExprNode) Children[0]).Generate(row);
            string s = "";
            foreach (var child in Children)
            {
                if (child is IExprNode)
                    s += ((IExprNode)child).Generate(row).String;
                else ((IVoidNode)child).Generate(row);
            }
            return new StringMean(s);
        }
    }
}