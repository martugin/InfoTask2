using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Цикл, возвращающий значение
    internal class NodeWhile : NodeKeeper, INodeExpr
    {
        public NodeWhile(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, INodeExpr prog, INodeExpr elseProg)
            : base(keeper, terminal)
        {
            _condition = condition;
            _prog = prog;
            _elseProg = elseProg;
        }

        protected override string NodeType { get { return "While"; } }

        //Условие, вычисляеое выражение и значение возвращаемое если цикл ни разу не выполнялся
        private readonly INodeExpr _condition;
        private readonly INodeExpr _prog;
        private readonly INodeExpr _elseProg;

        //Получение типа данных
        public DataType Check(TablStructItem row)
        {
            if (_condition.Check(row) != DataType.Boolean)
                AddError("Нодопустимый тип данных условия");
            return _prog.Check(row).Add(_elseProg.Check(row));
        }

        //Вычисление значения
        public Mean Process(SubRows row)
        {
            Mean mean = null;
            while (_condition.Process(row).Boolean)
                mean = _prog.Process(row);
            return mean ?? _elseProg.Process(row);
        }
    }

    //----------------------------------------------------------------------------------------------------------
    //Цикл, возвращающий значение
    internal class NodeWhileVoid : NodeKeeper, INodeVoid
    {
        public NodeWhileVoid(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, INodeVoid prog)
            : base(keeper, terminal)
        {
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "WhileVoid"; } }

        //Условие и выполняемая программа
        private readonly INodeExpr _condition;
        private readonly INodeVoid _prog;

        //Проверка корректности выражений генерации
        public void Check(TablStructItem row)
        {
            if (_condition.Check(row) != DataType.Boolean)
                AddError("Нодопустимый тип данных условия");
        }

        //Вычисление значения
        public void Process(SubRows row)
        {
            while (_condition.Process(row).Boolean)
                _prog.Process(row);
        }
    }
}