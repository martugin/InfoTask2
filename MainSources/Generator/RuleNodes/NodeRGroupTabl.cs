using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел - родительский ряд для таблицы
    internal class NodeRGroupTabl : NodeRTablBase
    {
        public NodeRGroupTabl(ParsingKeeper keeper, ITerminalNode terminal, NodeList fields)
            : base(keeper, terminal)
        {
            _fields = fields.Children.Select(node => node.Token.Text).ToArray();
        }

        //Массив полей группировки
        private readonly string[] _fields;

        //Тип узла
        protected override string NodeType { get { return "GroupTabl"; } }

        //Проверка выражения
        public override IRowStruct Check(TablsList dataTabls, TablStruct tablParent)
        {
            var gstruct = new RowGroupStruct(tablParent, _fields);
            foreach (var field in _fields)
            {
                if (!tablParent.Fields.ContainsKey(field))
                    AddError("Поле для группировки не найдено в таблице");
                else gstruct.Fields.Add(field, tablParent.Fields[field]);
            }
            return gstruct;
        }

        //Выбрать ряды для генерации
        public override IEnumerable<SubRows> SelectRows(TablsList dataTabls, IEnumerable<SubRows> parentRows)
        {
            var groupDic = new GroupDic();
            foreach (TablRow row in parentRows)
                groupDic.AddRow(row, _fields, -1);
            return groupDic.GetGroups();
        }
    }
}