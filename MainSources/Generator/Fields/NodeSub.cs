using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел прохода по подтаблице, возвращает значение
    internal class NodeSub : NodeExpr
    {
        public NodeSub(ITerminalNode terminal, NodeExpr condition, NodeExpr expr, NodeExpr separator, GeneratorKeeper keeper)
            : base(terminal)
        {
            _keeper = keeper;
            _condition = condition;
            _expr = expr;
            _separator = separator;
        }

        protected override string NodeType { get { return "Sub"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Условие фильтрации или имя типа
        private NodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private NodeExpr _expr;
        //Разделитель
        private NodeExpr _separator;
        
        public override Mean Process()
        {
            throw new System.NotImplementedException();
        }
    }

    //------------------------------------------------------------------------------------------------------------------------
    //Узел прохода по подтаблице, ничего не возвращает
    internal class NodeSubVoid : NodeVoid
    {
        public NodeSubVoid(ITerminalNode terminal, NodeExpr condition, NodeVoid prog, GeneratorKeeper keeper)
            : base(terminal)
        {
            _keeper = keeper;
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "SubVoid"; } }

        //Ссылка на Keeper
        private GeneratorKeeper _keeper;
        //Условие фильтрации или имя типа
        private NodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private NodeVoid _prog;
        
        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}