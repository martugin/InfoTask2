using System.Collections.Generic;
using System.Linq;
using CommonTypes;

namespace Generator
{
    //Главный узел генерации значения поля
    internal class NodeText : NodeList, INodeExpr
    {
        public NodeText(IEnumerable<INodeExpr> children) 
            : base(children) { }

        //Проверка формулы
        public DataType Check(TablStruct tabl)
        {
            var dtype = DataType.Value;
            foreach (INodeExpr child in Children)
                dtype = dtype.Add(child.Check(tabl));
            return dtype;
        }

        //Генерация значения
        public Mean Generate(SubRows row)
        {
            if (Children.Count() == 1)
                return ((INodeExpr) Children.First()).Generate(row);
            string s = "";
            foreach (INodeExpr child in Children)
                s += child.Generate(row).String;
            return new MeanString(s);
        }
    }
}