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

        //Полный текст, записываемый в скомпилированное выражение
        public string CompiledFullText()
        {
            return NodeType + "!" + CompiledText() + "!0;";
        } 
        public virtual string CompiledText() { return "";}
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
        
        //Тип данных
        public ITablikType Type { get; protected set; }

        public virtual void DefineType() { }

        //Полный текст, записываемый в скомпилированное выражение
        public string CompiledFullText()
        {
            return NodeType + "!" + CompiledText() + "!" + Args.Length + ";";
        }
        public virtual string CompiledText() { return ""; }
    }
}