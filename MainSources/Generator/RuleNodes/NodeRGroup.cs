using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел - родительский ряд для таблицы
    internal class NodeRGroup : NodeKeeper, INodeRQuery
    {
        public NodeRGroup(ParsingKeeper keeper, ITerminalNode terminal, NodeList fields)
            : base(keeper, terminal)
        {
            _fields = fields.Children.Select(node => node.Token.Text).ToArray();
        }

        //Массив полей группировки
        private readonly string[] _fields;

        //Тип узла
        protected override string NodeType { get { return "GroupTabl"; } }

        //Проверка выражения
        public ITablStruct Check(TablsList dataTabls, ITablStruct tablParent)
        {
            var gstruct = new RowGroupStruct((TablStruct)tablParent, _fields);
            foreach (var field in _fields)
            {
                if (!tablParent.Fields.ContainsKey(field))
                    AddError("Поле для группировки не найдено в таблице");
                else gstruct.Fields.Add(field, tablParent.Fields[field]);
            }
            return gstruct;
        }

        //Выбрать ряды для генерации
        public IEnumerable<SubRows> SelectRows(IEnumerable<SubRows> parentRows)
        {
            var groupDic = new GroupDic();
            foreach (TablRow row in parentRows)
                groupDic.AddRow(row, _fields, -1);
            return groupDic.GetGroups();
        }
    }
}