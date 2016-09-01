using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел перехода к таблице - родителю, возвращает значение
    internal class NodeOver : NodeKeeper, INodeExpr
    {
        public NodeOver(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr expr) 
            : base(keeper, terminal) 
        {
            _expr = expr;
        }

        protected override string NodeType { get { return "Over"; } }

        //Вычисляемое выражение
        private readonly INodeExpr _expr;

        //Получение типа данных
        public DataType Check(TablStructItem row)
        {
            if (row.Parent == null)
                AddError("Недопустимы переход к надтаблице");
            return _expr.Check(row.Parent);
        }

        //Вычисление значения
        public Mean Process(SubRows row)
        {
            return _expr.Process(row.Parent);
        }
    }

    //------------------------------------------------------------------------------------------------------
    //Узел перехода к таблице - родителю, ничего не возвращает 
    internal class NodeOverVoid : NodeKeeper, INodeVoid
    {
        public NodeOverVoid(ParsingKeeper keeper, ITerminalNode terminal, INodeVoid prog)
            : base(keeper, terminal)
        {
            _prog = prog;
        }

        protected override string NodeType { get { return "OverVoid"; } }

        //Вычисляемое выражение
        private INodeVoid _prog;

        //Проверка корректности выражений генерации
        public void Check(TablStructItem row)
        {
            if (row.Parent == null)
                AddError("Недопустимый переход к надтаблице");
            _prog.Check(row.Parent);
        }

        //Вычисление значения
        public void Process(SubRows row)
        {
            _prog.Process(row.Parent);
        }
    }
}