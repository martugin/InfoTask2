using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел перехода к таблице - родителю, возвращает значение
    internal class NodeOver : Node, INodeExpr
    {
        public NodeOver(ITerminalNode terminal, INodeExpr expr, GeneratorKeeper keeper) : base(terminal) 
        {
            _keeper = keeper;
            _expr = expr;
        }

        protected override string NodeType { get { return "Over"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Вычисляемое выражение
        private INodeExpr _expr;
        
        public Mean Process(TablRow row)
        {
            throw new System.NotImplementedException();
        }
    }

    //------------------------------------------------------------------------------------------------------
    //Узел перехода к таблице - родителю, ничего не возвращает 
    internal class NodeOverVoid : Node, INodeVoid
    {
        public NodeOverVoid(ITerminalNode terminal, INodeVoid prog, GeneratorKeeper keeper)
            : base(terminal)
        {
            _keeper = keeper;
            _prog = prog;
        }

        protected override string NodeType { get { return "OverVoid"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Вычисляемое выражение
        private INodeVoid _prog;

        public void Process(TablRow row)
        {
            throw new System.NotImplementedException();
        }
    }
}