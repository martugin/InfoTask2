using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Calculation;
using CompileLibrary;

namespace Generator
{
    //Узел - родительский ряд для таблицы
    internal class GroupNodeR : KeeperNode, INodeRQuery
    {
        public GroupNodeR(ParsingKeeper keeper, ITerminalNode terminal, IEnumerable<INode> fieldsNodes)
            : base(keeper, terminal)
        {
            _fieldsNodes = (fieldsNodes ?? new INode[0]).ToArray();
            _fields = _fieldsNodes.Select(node => node.Token.Text).ToArray();
        }

        //Массив полей группировки
        private readonly INode[] _fieldsNodes;
        private readonly string[] _fields;

        //Тип узла
        protected override string NodeType { get { return "Group"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(_fieldsNodes);
        }

        //Проверка выражения
        public ITablStruct Check(TablsList dataTabls, ITablStruct tablParent)
        {
            var gstruct = new RowGroupStruct((TablStruct)tablParent, _fields);
            foreach (var field in _fieldsNodes)
            {
                var s = ((ConstNode)field).Mean.String;
                if (!tablParent.Fields.ContainsKey(s))
                    Keeper.AddError("Поле для группировки не найдено в таблице", field.Token);
                else gstruct.Fields.Add(s, tablParent.Fields[s]);
            }
            return gstruct;
        }

        //Выбрать ряды для генерации
        public IEnumerable<SubRows> SelectRows(IEnumerable<SubRows> parentRows)
        {
            var groupDic = new GroupDic();
            foreach (TablRow row in parentRows)
                groupDic.AddRow(row, _fields, 0);
            return groupDic.GetGroups();
        }
    }
}