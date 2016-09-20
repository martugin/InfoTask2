using System.Collections.Generic;
using System.Linq;
using CommonTypes;

namespace Generator
{
    //Главный узел генерации значения поля
    internal class NodeTextList : NodeList, INodeExpr
    {
        public NodeTextList(IEnumerable<Node> children) 
            : base(children) { }

        //Проверка формулы
        public DataType Check(TablStruct tabl)
        {
            var dtype = DataType.Value;
            foreach (var child in Children)
                if (child is INodeExpr)
                    dtype = dtype.Add(((INodeExpr)child).Check(tabl));
            return dtype;
        }

        //Генерация значения
        public IMean Generate(SubRows row)
        {
            if (Children.Count() == 1)
                return ((INodeExpr) Children.First()).Generate(row);
            string s = "";
            foreach (var child in Children)
                if (child is INodeExpr)
                    s += ((INodeExpr)child).Generate(row).String;
            return new MeanString(s);
        }
    }
}