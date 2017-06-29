using System.Collections.Generic;
using System.Linq;

namespace CompileLibrary
{
    //Промежуточный узел, собирающий список узлов 
    public class ListNode<T> : Node where T : INode
    {
        public ListNode(IEnumerable<T> children)
        {
            Children = children.ToList();
        }
        
        protected override string NodeType { get { return "NodeList"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Children.Cast<INode>().ToArray());
        }

        // Список узлов 
        public List<T> Children { get; private set; }
    }
}