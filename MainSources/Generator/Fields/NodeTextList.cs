using System.Collections.Generic;
using CommonTypes;

namespace Generator
{
    //Главный узел генерации значения поля
    internal class NodeTextList : NodeList, INodeExpr
    {
        public NodeTextList(IEnumerable<Node> children)
            : base(children) { }

        //Проверка формулы
        public DataType Check(ITablStruct tabl)
        {
            var dtype = DataType.Value;
            foreach (var child in Children)
                if (child is INodeExpr)
                {
                    var res = ((INodeExpr)child).Check(tabl);
                    dtype = dtype == DataType.Value ? res : DataType.String;
                } 
                else if (child is INodeVoid)
                    ((INodeVoid)child).Check(tabl);
            return dtype;
        }

        //Генерация значения
        public IMean Generate(SubRows row)
        {
            if (Children.Count == 1)
                return ((INodeExpr) Children[0]).Generate(row);
            string s = "";
            foreach (var child in Children)
            {
                if (child is INodeExpr)
                    s += ((INodeExpr)child).Generate(row).String;
                else ((INodeVoid)child).Generate(row);
            }
            return new MeanString(s);
        }
    }
}