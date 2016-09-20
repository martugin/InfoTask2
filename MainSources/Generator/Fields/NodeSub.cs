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
            _rowsSelector = new RowsSelector(keeper);
        }

        protected override string NodeType { get { return "Sub"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, _expr, _separator);
        }

        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private readonly INodeExpr _expr;
        //Разделитель
        private readonly INodeExpr _separator;

        //Фильтрация списка рядов подтаблицы
        private readonly RowsSelector _rowsSelector;

        //Проверка корректности выражений генерации
        public DataType Check(TablStruct tabl)
        {
            if (tabl.Child == null)
            {
                AddError("Недопустимый переход к подтаблице");
                return DataType.Error;
            }
            _rowsSelector.Check(_condition, tabl.Child);
            _expr.Check(tabl.Child);
            if (_separator != null)
                _separator.Check(tabl.Child);
            return DataType.String;
        }

        //Вычисление значения по ряду исходной таблицы
        public IMean Generate(SubRows row)
        {
            _rowsSelector.SelectRows(_condition, row);
            string s = "";
            foreach (var r in _rowsSelector.SelectRows(_condition, row))
            {
                if (s != "" && _separator != null)
                    s += _separator.Generate(r).String;
                s += _expr.Generate(r).String;
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
            _rowsSelector = new RowsSelector(keeper);
        }

        protected override string NodeType { get { return "SubVoid"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, _prog);
        }

        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;
        //Выражение, вычисяемое для каждой строки подтаблицы
        private readonly INodeVoid _prog;
        //Фильтрация списка рядов подтаблицы
        private readonly RowsSelector _rowsSelector;

        //Проверка корректности выражений генерации
        public void Check(TablStruct tabl)
        {
            if (tabl.Child == null)
                AddError("Недопустимый переход к подтаблице");
            else if (_condition != null && _condition.Check(tabl.Child) != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            else _prog.Check(tabl.Child);
        }

        //Вычисление значения по ряду исходной таблицы
        public void Generate(SubRows row)
        {
            foreach (var r in _rowsSelector.SelectRows(_condition, row))
                _prog.Generate(r);
        }
    }
}