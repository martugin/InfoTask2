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
        public override string ToTestString()
        {
            return ToTestWithChildren(_expr);
        }

        //Вычисляемое выражение
        private readonly INodeExpr _expr;

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            if (tabl.Parent == null)
            {
                AddError("Недопустимый переход к надтаблице");
                return DataType.Error;
            }
            return _expr.Check(tabl.Parent);
        }

        //Вычисление значения
        public IMean Generate(SubRows row)
        {
            return _expr.Generate(row.Parent);
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

        public override string ToTestString()
        {
            return ToTestWithChildren(_prog);
        }

        //Вычисляемое выражение
        private readonly INodeVoid _prog;

        //Проверка корректности выражений генерации
        public void Check(TablStruct tabl)
        {
            if (tabl.Parent == null)
                AddError("Недопустимый переход к надтаблице");
            else _prog.Check(tabl.Parent);
        }

        //Вычисление значения
        public void Generate(SubRows row)
        {
            _prog.Generate(row.Parent);
        }
    }
}