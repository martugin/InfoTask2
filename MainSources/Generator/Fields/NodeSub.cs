using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел прохода по подтаблице, возвращает значение
    internal class NodeSub : Node, INodeExpr
    {
        public NodeSub(ITerminalNode terminal, INodeExpr condition, INodeExpr expr, INodeExpr separator, GeneratorKeeper keeper)
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
        private INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private INodeExpr _expr;
        //Разделитель
        private INodeExpr _separator;
        
        public Mean Process(TablRow row)
        {
            throw new System.NotImplementedException();
        }
    }

    //------------------------------------------------------------------------------------------------------------------------
    //Узел прохода по подтаблице, ничего не возвращает
    internal class NodeSubVoid : Node, INodeVoid
    {
        public NodeSubVoid(ITerminalNode terminal, INodeExpr condition, INodeVoid prog, GeneratorKeeper keeper)
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
        private INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private INodeVoid _prog;
        
        public void Process(TablRow row)
        {
            throw new System.NotImplementedException();
        }
    }
}