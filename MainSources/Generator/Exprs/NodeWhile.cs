using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Цикл, возвращающий значение
    internal class NodeWhile : NodeKeeper, INodeExpr
    {
        public NodeWhile(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, INodeExpr prog, INodeExpr elseProg)
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
        private readonly INodeExpr _condition;
        private readonly INodeExpr _prog;
        private readonly INodeExpr _elseProg;

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            if (_condition.Check(tabl) != DataType.Boolean)
            {
                AddError("Недопустимый тип данных условия");
                return DataType.Error;
            }
            return _prog.Check(tabl).Add(_elseProg.Check(tabl));
        }

        //Вычисление значения
        public IMean Generate(SubRows row)
        {
            IMean mean = null;
            while (_condition.Generate(row).Boolean)
                mean = _prog.Generate(row);
            return mean ?? _elseProg.Generate(row);
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

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, _prog);
        }

        //Условие и выполняемая программа
        private readonly INodeExpr _condition;
        private readonly INodeVoid _prog;

        //Проверка корректности выражений генерации
        public void Check(TablStruct tabl)
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