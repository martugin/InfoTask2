using System.Collections.Generic;
using System.Text;
using CompileLibrary;

namespace Tablik
{
    //Узел, задающий список узлов
    internal class TablikListNode : ListNode<IExprNode>, IExprNode
    {
        public TablikListNode(IEnumerable<IExprNode> children) 
            : base(children) { }

        public TablikListNode(params IExprNode[] nodes) 
            : base(nodes) { }

        //Тип данных
        public ITablikType Type { get { return null; } }
        public void DefineType() { }

        //Записать текст в скомпилированное выражение
        public void SaveCompiled(StringBuilder sb)
        {
            foreach (var node in Nodes)
                node.SaveCompiled(sb);
        }
    }
}