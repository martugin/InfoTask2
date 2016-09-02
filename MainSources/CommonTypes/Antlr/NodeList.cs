using System.Collections.Generic;

namespace CommonTypes
{
    //Промежуточный узел, собирающий список узлов 
    public class NodeList : Node
    {
        public NodeList(IEnumerable<INode> children) : base(null)
        {
            Children = children;
        }

        protected override string NodeType { get { return "NodeList"; } }

        // Список узлов 
        public IEnumerable<INode> Children { get; private set; }
    }
}