using System.Collections.Generic;
using CompileLibrary;

namespace Tablik
{
    //Узел, задающий список узлов
    internal class ListExprNode : ListNode<IExprNode>, IExprNode
    {
        public ListExprNode(IEnumerable<IExprNode> children) 
            : base(children) { }

        public ListExprNode(params IExprNode[] nodes) 
            : base(nodes) { }

        //Тип данных
        public ITablikType Type { get { return null; } }
        public void DefineType() { }
        public string CompiledFullText() { return ""; }
    }
}