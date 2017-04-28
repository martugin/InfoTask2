using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Calculation;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Узел - перебор рядов подтаблицы
    internal class SubNodeR : KeeperNode, INodeRTabl, INodeRQuery
    {
        public SubNodeR(ParsingKeeper keeper, ITerminalNode terminal, IExprNode condition)
            : base(keeper, terminal)
        {
            Condition = condition;
        }

        //Условие фильтрации или имя типа
        protected IExprNode Condition { get; private set; }
        //Следующий узел в цепочке
        public INodeRQuery ChildNode { get; set; }

        //Тип узла
        protected override string NodeType { get { return "SubTabl"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(Condition, ChildNode);
        }

        //Проверка выражения
        public ITablStruct Check(TablsList dataTabls, ITablStruct parentStruct)
        {
            if (parentStruct.Child == null)
            {
                AddError("Подтаблица отстутствует");
                return null;
            }
            if (Condition != null && Condition.Check(parentStruct.Child) != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            return ChildNode == null ? parentStruct.Child : ChildNode.Check(dataTabls, parentStruct.Child);
        }

        //Выбрать ряды для генерации, главный узел выражения подтаблицы
        public IEnumerable<SubRows> SelectRows(TablsList dataTabls, SubRows parentRow)
        {
            IEnumerable<SubRows> rows = Condition == null
                                            ? parentRow.SubList
                                            : parentRow.SubList.Where(row => Condition.Generate(row).Boolean);
            return ChildNode == null ? rows : ChildNode.SelectRows(rows);
        }

        //Выбрать ряды для генерации, узел запроса
        public IEnumerable<SubRows> SelectRows(IEnumerable<SubRows> parentRows)
        {
            var rows = parentRows.SelectMany(row => Condition == null 
                                                                               ? row.SubList 
                                                                               : row.SubList.Where(r => Condition.Generate(r).Boolean));
            return ChildNode == null ? rows : ChildNode.SelectRows(rows);
        }
    }
}