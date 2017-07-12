using System.Text;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Базовый класс для узлов
    internal abstract class TablikNode : Node, IExprNode
    {
        protected TablikNode(ITerminalNode terminal) 
            : base(terminal) { }

        //Тип данных
        public ITablikType Type { get; protected set; }

        public virtual void DefineType() { }

        //Записать текст в скомпилированное выражение
        public void SaveCompiled(StringBuilder sb)
        {
            sb.Append(NodeType).Append("!").Append(CompiledText()).Append(";");
        }

        public virtual string CompiledText()
        {
            return Token == null ? "" : Token.Text;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Базовый класс для узлов с аргументами и накопителм ошибок
    internal abstract class TablikKeeperNode : KeeperNode, IExprNode
    {
        //Тип данных
        protected TablikKeeperNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args) 
            : base(keeper, terminal)
        {
            Args = args;
        }

        //Аргументы
        public IExprNode[] Args { get; protected set; }
        //Является взятием метода
        public bool IsMet { get; protected set; }
        
        //Тип данных
        public ITablikType Type { get; protected set; }

        public virtual void DefineType() { }

        //Записать текст в скомпилированное выражение
        public void SaveCompiled(StringBuilder sb)
        {
            foreach (var arg in Args)
                arg.SaveCompiled(sb);
            sb.Append(NodeType).Append("!").Append(CompiledText()).Append("!").Append(Args.Length + (IsMet ? 1 : 0)).Append(";");
        }
        public virtual string CompiledText()
        {
            return Token == null ? "" : Token.Text;
        }
    }
}