using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел прохода по подтаблице, возвращает значение
    internal class NodeSub : NodeKeeper, INodeExpr
    {
        public NodeSub(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, INodeExpr expr, INodeExpr separator)
            : base(keeper, terminal)
        {
            _condition = condition;
            _expr = expr;
            _separator = separator;
            _rowsChecker = new RowsConditionChecker(keeper);
        }

        protected override string NodeType { get { return "Sub"; } }

        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private readonly INodeExpr _expr;
        //Разделитель
        private readonly INodeExpr _separator;

        //Фильтрация списка рядов подтаблицы
        private readonly RowsConditionChecker _rowsChecker;

        //Проверка корректности выражений генерации
        public DataType Check(TablStructItem row)
        {
            if (row.Child == null)
                AddError("Недопустимый переход к подтаблице");
            _rowsChecker.Check(_condition, row.Child);
            _expr.Check(row.Child);
            if (_separator != null)
                _separator.Check(row.Child);
            return DataType.String;
        }

        //Вычисление значения по ряду исходной таблицы
        public Mean Process(SubRows row)
        {
            _rowsChecker.GetInitialRows(_condition, row);
            string s = "";
            foreach (var r in _rowsChecker.GetInitialRows(_condition, row))
            {
                if (s != "" && _separator != null)
                    s += _separator.Process(r).String;
                s += _expr.Process(r).String;
            }
            return new MeanString(s);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------
    //Узел прохода по подтаблице, ничего не возвращает
    internal class NodeSubVoid : NodeKeeper, INodeVoid
    {
        public NodeSubVoid(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, INodeVoid prog)
            : base(keeper, terminal)
        {
            _condition = condition;
            _prog = prog;
        }

        protected override string NodeType { get { return "SubVoid"; } }

        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private readonly INodeVoid _prog;

        //Проверка корректности выражений генерации
        public void Check(TablStructItem row)
        {
            if (row.Child == null)
                AddError("Недопустимый переход к подтаблице");
            if (_condition != null && _condition.Check(row.Child) != DataType.Boolean)
                AddError("Нодопустимый тип данных условия");
            _prog.Check(row.Child);
        }

        //Вычисление значения по ряду исходной таблицы
        public void Process(SubRows row)
        {
            IEnumerable<TablRow> rows = row.SubList;
            if (_condition != null)
            {
                if (_condition is NodeConst && row.Parent.SubTypes.ContainsKey(((NodeConst)_condition).Mean.String))
                    rows = row.Parent.SubTypes[((NodeConst)_condition).Mean.String];
                else rows = row.Parent.SubList.Where(r => _condition.Process(r).Boolean);
            }
            foreach (var r in rows)
                _prog.Process(r);
        }
    }
}