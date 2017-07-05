using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий обращение к таблице
    internal class TablNode : TablikKeeperNode
    {
        public TablNode(TablikKeeper keeper, ITerminalNode tabl, ITerminalNode field, params IExprNode[] args) 
            : base(keeper, tabl, args)
        {
        }


    }
}