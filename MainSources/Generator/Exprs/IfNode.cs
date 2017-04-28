using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Calculation;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Условие, возвращающее значение
    internal class IfNode : KeeperNode, IExprNode
    {
        public IfNode(ParsingKeeper keeper, ITerminalNode terminal, List<IExprNode> conditions, List<IExprNode> variants)
            : base(keeper, terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "If"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_conditions.Union(_variants).ToArray());
        }

        //Список условий
        private readonly List<IExprNode> _conditions;
        //Список вариантов значений
        private readonly List<IExprNode> _variants;

        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            if (_conditions.Any(c => c.Check(tabl) != DataType.Boolean))
            {
                AddError("Недопустимый тип данных условия");
                return DataType.Error;
            }
            var dt = DataType.Value;
            foreach (var expr in _variants)
                dt = dt.Add(expr.Check(tabl));
            return dt;
        }

        //Вычисление значения
        public IReadMean Generate(SubRows row)
        {
            for (int i = 0; i < _variants.Count - 1; i++)
                if (_conditions[i].Generate(row).Boolean)
                    return _variants[i].Generate(row);
            return _variants.Last().Generate(row);
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //Условие, возвращающее значение
    internal class NodeIfVoid : KeeperNode, IVoidNode
    {
        public NodeIfVoid(ParsingKeeper keeper, ITerminalNode terminal, List<IExprNode> conditions, List<IVoidNode> variants)
            : base(keeper, terminal)
        {
            _conditions = conditions;
            _variants = variants;
        }

        protected override string NodeType { get { return "IfVoid"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_conditions.Cast<Node>().Union(_variants.Cast<Node>()).ToArray());
        }

        //Список условий
        private readonly List<IExprNode> _conditions;
        //Список вариантов значений
        private readonly List<IVoidNode> _variants;

        //Проверка корректности выражений генерации
        public void Check(ITablStruct tabl)
        {
            if (_conditions.Any(c => c.Check(tabl) != DataType.Boolean))
                AddError("Недопустимый тип данных условия");
            foreach (var expr in _variants)
                expr.Check(tabl);
        }

        //Вычисление значения
        public void Generate(SubRows row)
        {
            for (int i = 0; i < _variants.Count; i++)
                if (i == _conditions.Count || _conditions[i].Generate(row).Boolean)
                    _variants[i].Generate(row);
        }
    }
}