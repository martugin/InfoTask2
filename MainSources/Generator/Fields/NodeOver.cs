using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел перехода к таблице - родителю, возвращает значение
    internal class NodeOver : NodeExpr
    {
        public NodeOver(ITerminalNode terminal, NodeExpr expr, GeneratorKeeper keeper) : base(terminal) 
        {
            _keeper = keeper;
            _expr = expr;
        }

        protected override string NodeType { get { return "Over"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Вычисляемое выражение
        private NodeExpr _expr;
        
        public override Mean Process()
        {
            throw new System.NotImplementedException();
        }
    }

    //------------------------------------------------------------------------------------------------------
    //Узел перехода к таблице - родителю, ничего не возвращает 
    internal class NodeOverVoid : NodeVoid
    {
        public NodeOverVoid(ITerminalNode terminal, NodeVoid prog, GeneratorKeeper keeper)
            : base(terminal)
        {
            _keeper = keeper;
            _prog = prog;
        }

        protected override string NodeType { get { return "OverVoid"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Вычисляемое выражение
        private NodeVoid _prog;

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}