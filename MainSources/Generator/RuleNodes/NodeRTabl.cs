using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Узел - проход по таблице
    internal class NodeRTabl : KeeperNode, INodeRTabl
    {
        public NodeRTabl(GenKeeper keeper, ITerminalNode terminal, IExprNode condition)
            : base(keeper, terminal)
        {
            if (terminal != null)
                _tablName = terminal.Symbol.Text;
            _condition = condition;
        }

        //Имя таблицы
        private readonly string _tablName;
        //Условие фильтрации или имя типа
        private readonly IExprNode _condition;
        //Следующий узел в цепочке
        public INodeRQuery ChildNode { get; set; }

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, ChildNode);
        }

        //Проверка выражения
        public ITablStruct Check(TablsList dataTabls, ITablStruct parentStruct)
        {
            if (!dataTabls.Structs.ContainsKey(_tablName))
            {
                AddError("Не найдена таблица");
                return null;
            }
            var tstruct = dataTabls.Structs[_tablName].Tabls[0];
            if (_condition != null && _condition.Check(tstruct) != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            return ChildNode == null ? tstruct : ChildNode.Check(dataTabls, tstruct);
        }

        //Выбрать ряды для генерации
        public IEnumerable<SubRows> SelectRows(TablsList dataTabls, SubRows parentRow)
        {
            IEnumerable<SubRows> rows = dataTabls.Tabls[_tablName].SubList;
            rows = _condition == null ? rows : rows.Where(row => _condition.Generate(row).Boolean);
            return ChildNode == null ? rows : ChildNode.SelectRows(rows);
        }
    }
}