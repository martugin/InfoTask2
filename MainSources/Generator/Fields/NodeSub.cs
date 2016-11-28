using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел прохода по подтаблице, возвращает значение
    internal class NodeSub : NodeKeeper, INodeExpr
    {
        public NodeSub(ParsingKeeper keeper, ITerminalNode terminal,
                                 IEnumerable<INodeExpr> pars) //Параметры, смысл каждого параметра определяется при проверке
            : base(keeper, terminal)
        {
            _pars = pars.ToArray();
        }

        protected override string NodeType { get { return "Sub"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_pars);
        }

        //Узлы - параметры функции SubTabl
        private readonly INodeExpr[] _pars;

        //Условие фильтрации или имя типа
        private INodeExpr _condition;
        //Выражение, вычисляемое для каждой строки подтаблицы
        private INodeExpr _expr;
        //Разделитель
        private INodeExpr _separator;

        //Проверка корректности выражений генерации
        public DataType Check(ITablStruct tabl)
        {
            var child = tabl.Child;
            if (child == null)
            {
                AddError("Недопустимый переход к подтаблице");
                return DataType.Error;
            }
            if (_pars.Length > 1 && _pars[0].Check(child) == DataType.Boolean)
            {
                _condition = _pars[0];
                _expr = _pars[1];
                if (_pars.Length == 3) _separator = _pars[2];
            }
            else
            {
                _expr = _pars[0];
                if (_pars.Length == 2) _separator = _pars[1];
                if (_pars.Length == 3)
                    AddError("Недопустимый тип данных условия");
            }

            _expr.Check(child);
            if (_separator != null)
                _separator.Check(child);
            return DataType.String;
        }

        //Вычисление значения по ряду исходной таблицы
        public IMean Generate(SubRows row)
        {
            IEnumerable<SubRows> rows = row.SubList;
            if (_condition != null)
                rows = rows.Where(r => _condition.Generate(r).Boolean);
            string s = "";
            foreach (var r in rows)
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

        //Проверка корректности выражений генерации
        public void Check(ITablStruct tabl)
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
            IEnumerable<SubRows> rows = row.SubList;
            if (_condition != null)
                rows = rows.Where(r => _condition.Generate(r).Boolean);
            foreach (var r in rows)
                _prog.Generate(r);
        }
    }
}