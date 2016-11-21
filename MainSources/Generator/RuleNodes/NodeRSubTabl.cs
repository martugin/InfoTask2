using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел - перебор рядов подтаблицы
    internal class NodeRSubTabl : NodeRTablBase
    {
        public NodeRSubTabl(ParsingKeeper keeper, ITerminalNode terminal, INodeExpr condition, NodeRTablBase childNode)
            : base(keeper, terminal, childNode)
        {
            _condition = condition;
        }

        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;

        //Тип узла
        protected override string NodeType { get { return "SubTabl"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_condition, ChildNode);
        }

        //Проверка выражения
        public override IRowStruct Check(TablsList dataTabls, TablStruct parentStruct)
        {
            if (parentStruct.Child == null)
            {
                AddError("Подтаблица отстутствует");
                return null;
            }
            if (_condition != null && _condition.Check(parentStruct.Child) != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            return ChildNode == null ? parentStruct.Child : ChildNode.Check(dataTabls, parentStruct.Child);
        }

        //Выбрать ряды для генерации
        public override IEnumerable<SubRows> SelectRows(TablsList dataTabls, IEnumerable<SubRows> parentRows)
        {
            if (_condition == null)
                return parentRows.SelectMany(row => row.SubList);
            return parentRows.SelectMany(row => row.SubList.Where(r => _condition.Generate(r).Boolean));
        }
    }
}