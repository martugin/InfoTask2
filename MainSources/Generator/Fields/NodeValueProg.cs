using CommonTypes;

namespace Generator
{
    //Программа без значения
    internal class NodeVoidProg : Node, INodeVoid
    {
        public NodeVoidProg(params INodeVoid[] parts) : base(null)
        {
            _voidParts = parts;
        }

        protected override string NodeType { get { return "VoidProg"; } }

        //Список частей программы
        private readonly INodeVoid[] _voidParts;

        //Проверка корректности выражений генерации
        public void Check(TablStruct tabl)
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
    internal class NodeValueProg : Node, INodeExpr
    {
        public NodeValueProg(NodeVoidProg voidProg, INodeExpr expr) : base(null)
        {
            _voidProg = voidProg;
            _expr = expr;
        }

        //Узел, список частей программы без значения
        private readonly NodeVoidProg _voidProg;
        //Выражение, возвращающее значение
        private readonly INodeExpr _expr;

        protected override string NodeType { get { return "ValueProg"; } }

        //Проверка корректности выражений генерации, определение типа данных выражения
        public DataType Check(TablStruct tabl)
        {
            _voidProg.Check(tabl);
            return _expr.Check(tabl);
        }

        //Вычисление значения по ряду исходной таблицы
        public IMean Generate(SubRows row)
        {
            _voidProg.Generate(row);
            return _expr.Generate(row);
        }
    }
}