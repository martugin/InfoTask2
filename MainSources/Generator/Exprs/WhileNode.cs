using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Цикл, возвращающий значение
    internal class WhileNode : KeeperNode, IExprNode
    {
        public WhileNode(ParsingKeeper keeper, ITerminalNode terminal, IExprNode condition, IExprNode prog, IExprNode elseProg)
            : base(keeper,terminal)
        {
            _condition = condition;
            _prog = prog;
            _elseProg = elseProg;
        }

        protected override string NodeType { get { return "While"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, _prog, _elseProg);
        }

        //Условие, вычисляеое выражение и значение возвращаемое если цикл ни разу не выполнялся
        private readonly IExprNode _condition;
        private readonly IExprNode _prog;
        private readonly IExprNode _elseProg;

        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            if (_condition.Check(tabl) != DataType.Boolean)
            {
                AddError("Недопустимый тип данных условия");
                return DataType.Error;
            }
            return _prog.Check(tabl).Add(_elseProg.Check(tabl));
        }

        //Вычисление значения
        public IReadMean Generate(SubRows row)
        {
            IReadMean mean = null;
            while (_condition.Generate(row).Boolean)
                mean = _prog.Generate(row);
            return mean ?? _elseProg.Generate(row);
        }
    }

    //----------------------------------------------------------------------------------------------------------
    //Цикл, возвращающий значение
    internal class NodeWhileVoid : KeeperNode, IVoidNode
    {
        public NodeWhileVoid(ParsingKeeper keeper, ITerminalNode terminal, IExprNode condition, IVoidNode prog)
            : base(keeper, terminal)
        {
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "WhileVoid"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, _prog);
        }

        //Условие и выполняемая программа
        private readonly IExprNode _condition;
        private readonly IVoidNode _prog;

        //Проверка корректности выражений генерации
        public void Check(ITablStruct tabl)
        {
            if (_condition.Check(tabl) != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            _prog.Check(tabl);
        }

        //Вычисление значения
        public void Generate(SubRows row)
        {
            while (_condition.Generate(row).Boolean)
                _prog.Generate(row);
        }
    }
}