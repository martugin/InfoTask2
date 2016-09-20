using System.Collections.Generic;
using System.Linq;

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

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Children.ToArray());
        }

        // Список узлов 
        public IEnumerable<INode> Children { get; private set; }
    }
}