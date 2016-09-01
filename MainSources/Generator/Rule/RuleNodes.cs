using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узлы разбора правила генерации

    //Узел - вызывающий итерацию по строкам таблицы, базовый для Tabl, OverTabl
    internal interface INodeTabl
    {
        //Проверка выражения, возвращает уровень таблицы, ряды которого перебираются
        TablStructItem Check(TablsList dataTabls);
        //Сгененирировать таблицу по исходным данным
        IEnumerable<SubRows> GetInitialRows(TablsList dataTabls);
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - родительский ряд для таблицы
    internal class NodeROverTabl : NodeKeeper, INodeTabl
    {
        public NodeROverTabl(ParsingKeeper keeper, ITerminalNode tabl)
            : base(keeper, tabl)
        {
            if (tabl != null)
                _tablName = tabl.Symbol.Text;
        }

        //Имя таблицы
        private readonly string _tablName;

        //Тип узла
        protected override string NodeType { get { return "OverTabl"; } }

        //Проверка выражения
        public TablStructItem Check(TablsList dataTabls)
        {
            if (!dataTabls.Tabls.ContainsKey(_tablName))
            {
                AddError("Не найдена таблица");
                return null;
            }
            return dataTabls.Structs[_tablName].Tabls[-1];
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(TablsList dataTabls)
        {
            return new[] { dataTabls.Tabls[_tablName].Parent };
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - перебор рядов подтаблицы
    internal class NodeRSubTabl : NodeKeeper
    {
        public NodeRSubTabl(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition = null)
            : base(keeper, terminal)
        {
            Condition = condition;
            _rowsChecker = new RowsConditionChecker(Keeper);
        }

        //Условие фильтрации или имя типа
        protected INodeExpr Condition { get; private set; }
        //Выбор рядов из дочерней подтаблицы
        internal protected NodeRSubTabl Child { get; set; }
        //Фильтрация списка рядов подтаблицы
        private readonly RowsConditionChecker _rowsChecker;
        
        //Тип узла
        protected override string NodeType { get { return "SubTabl"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(Condition, Child);
        }

        //Проверка выражения
        public TablStructItem Check(TablStructItem row) //таблица уровня родителя
        {
            _rowsChecker.Check(Condition, row);
            if (Child != null) return Child.Check(row.Child);
            return row;
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(SubRows parent)
        {
            return _rowsChecker.GetInitialRows(Condition, parent);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    //Узел - проход по таблице
    internal class NodeRTabl : NodeRSubTabl, INodeTabl
    {
        public NodeRTabl(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition = null)
            : base(keeper, terminal, condition)
        {
            if (terminal != null)
                _tablName = terminal.Symbol.Text;
        }

        //Имя таблицы
        private readonly string _tablName;

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }

        //Проверка выражения
        public TablStructItem Check(TablsList dataTabls)
        {
            var row = dataTabls.Structs[_tablName].Tabls[0];
            return Check(row);
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(TablsList dataTabls)
        {
            return GetInitialRows(dataTabls.Tabls[_tablName].Parent);
        }
    }
}