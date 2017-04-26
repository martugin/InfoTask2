using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CompileLibrary
{
    //Узел выражения, разбираемого ANTLR
    public abstract class Node : INode
    {
        protected Node(ITerminalNode terminal)
        {
            if (terminal != null)
                Token = terminal.Symbol;
        }
        
        //Ссылка на токен
        public IToken Token { get; private set; }
        //Тип узла, для записи в строку
        protected abstract string NodeType { get; }

        //Запись в строку
        public virtual string ToTestString()
        {
            return ToTestWithChildren();
        }

        //Запись в строку, с указанием списка детей
        protected string ToTestWithChildren(params INode[] children)
        {
            var sb = new StringBuilder(NodeType + ": ");
            if (Token != null) sb.Append(Token.Text);
            if (children.Length != 0)
            {
                if (Token != null) sb.Append(" ");
                sb.Append("(");
                for (int i = 0; i < children.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    if (children[i] != null) sb.Append(children[i].ToTestString());
                }
                sb.Append(")");
            }
            return sb.ToString();
        }
    }
}