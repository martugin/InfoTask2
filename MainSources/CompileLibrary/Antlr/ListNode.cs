using System.Collections.Generic;
using System.Linq;

namespace CompileLibrary
{
    //Промежуточный узел, собирающий список узлов 
    public class ListNode<T> : Node where T : INode
    {
        public ListNode(IEnumerable<T> children)
        {
            Nodes = children.ToArray();
        }

        protected override string NodeType { get { return "NodeList"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Nodes.Cast<INode>().ToArray());
        }

        // Список узлов 
        public T[] Nodes { get; private set; }
    }

    //-------------------------------------------------------------------------------
    //Список узлов типа Node
    public class ListNode : ListNode<Node>
    {
        public ListNode(IEnumerable<Node> children)
            : base(children) { }
    }
}