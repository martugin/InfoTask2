using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Условие, возвращающее значение
    internal class NodeIf : NodeKeeper, INodeExpr
    {
        public NodeIf(ParsingKeeper keeper, ITerminalNode terminal, List<INodeExpr> conditions, List<INodeExpr> variants)
            : base(keeper, terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "If"; } }

        //Список условий
        private readonly List<INodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<INodeExpr> _variants;

        //Получение типа данных
        public DataType Check(TablStructItem row)
        {
            if (_conditions.Any(c => c.Check(row) != DataType.Boolean))
                AddError("Нодопустимый тип данных условия");
            var dt = DataType.Value;
            foreach (var expr in _variants)
                dt = dt.Add(expr.Check(row));
            return dt;
        }

        //Вычисление значения
        public Mean Process(SubRows row)
        {
            for (int i = 0; i < _variants.Count - 1; i++)
                if (_conditions[i].Process(row).Boolean)
                    return _variants[i].Process(row);
            return _variants.Last().Process(row);
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //Условие, возвращающее значение
    internal class NodeIfVoid : NodeKeeper, INodeVoid
    {
        public NodeIfVoid(ParsingKeeper keeper, ITerminalNode terminal, List<INodeExpr> conditions, List<INodeVoid> variants)
            : base(keeper, terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "IfVoid"; } }

        //Список условий
        private readonly List<INodeExpr> _conditions;
        //Список вариантов значений
        private readonly List<INodeVoid> _variants;

        //Проверка корректности выражений генерации
        public void Check(TablStructItem row)
        {
            if (_conditions.Any(c => c.Check(row) != DataType.Boolean))
                AddError("Нодопустимый тип данных условия");
        }

        //Вычисление значения
        public void Process(SubRows row)
        {
            for (int i = 0; i < _variants.Count; i++)
                if (i == _conditions.Count || _conditions[i].Process(row).Boolean)
                    _variants[i].Process(row);
        }
    }
}