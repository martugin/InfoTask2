using Antlr4.Runtime.Tree;
using Calculation;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Узел перехода к таблице - родителю, возвращает значение
    internal class OverNode : KeeperNode, IExprNode
    {
        public OverNode(GenKeeper keeper, ITerminalNode terminal, IExprNode expr) 
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
        private readonly IExprNode _expr;

        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            if (tabl is RowGroupStruct)
                AddError("Переход к надтаблице недопустим для сгруппированных строк");
            else if (tabl.Parent == null)
                AddError("Недопустимый переход к надтаблице");
            else return _expr.Check(tabl.Parent);
            return DataType.Error;
        }

        //Вычисление значения
        public IReadMean Generate(SubRows row)
        {
            return _expr.Generate(row.Parent);
        }
    }

    //------------------------------------------------------------------------------------------------------
    //Узел перехода к таблице - родителю, ничего не возвращает 
    internal class OverVoidNode : KeeperNode, IVoidNode
    {
        public OverVoidNode(GenKeeper keeper, ITerminalNode terminal, IVoidNode prog)
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
        private readonly IVoidNode _prog;

        //Проверка корректности выражений генерации
        public void Check(ITablStruct tabl)
        {
            if (tabl is RowGroupStruct)
                AddError("Переход к надтаблице недопустим для сгруппированных строк");
            else if (tabl.Parent == null)
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