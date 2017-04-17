using CommonTypes;

namespace Generator
{
    //Программа без значения
    internal class NodeVoidProg : Node, IVoidNode
    {
        public NodeVoidProg(params IVoidNode[] parts) : base(null)
        {
            _voidParts = parts;
        }

        protected override string NodeType { get { return "VoidProg"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_voidParts);
        }

        //Список частей программы
        private readonly IVoidNode[] _voidParts;

        //Проверка корректности выражений генерации
        public void Check(ITablStruct tabl)
        {
            foreach (var part in _voidParts)
                part.Check(tabl);
        }

        //Выполнение действий по ряду исходной таблицы
        public void Generate(SubRows row)
        {
            foreach (var part in _voidParts)
                part.Generate(row);
        }
    }

    //-------------------------------------------------------------------------------------------------------------
    //Программа, возвращающая значение
    internal class NodeValueProg : Node, IExprNode
    {
        public NodeValueProg(NodeVoidProg voidProg, IExprNode expr) : base(null)
        {
            _voidProg = voidProg;
            _expr = expr;
        }

        //Узел, список частей программы без значения
        private readonly NodeVoidProg _voidProg;
        //Выражение, возвращающее значение
        private readonly IExprNode _expr;

        public override string ToTestString()
        {
            return ToTestWithChildren(_voidProg, _expr);
        }

        protected override string NodeType { get { return "ValueProg"; } }

        //Проверка корректности выражений генерации, определение типа данных выражения
        public DataType Check(ITablStruct tabl)
        {
            _voidProg.Check(tabl);
            return _expr.Check(tabl);
        }

        //Вычисление значения по ряду исходной таблицы
        public IReadMean Generate(SubRows row)
        {
            _voidProg.Generate(row);
            return _expr.Generate(row);
        }
    }
}