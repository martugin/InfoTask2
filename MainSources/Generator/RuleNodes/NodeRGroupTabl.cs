using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using BaseLibrary;
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
        public override void Check(TablsList dataTabls, TablStruct tablParent)
        {
            foreach (var field in _fields)
                if (!tablParent.Fields.ContainsKey(field))
                    AddError("Поле для группировки не найдено в таблице");
        }

        //Выбрать ряды для генерации
        public override IEnumerable<SubRows> SelectRows(TablsList dataTabls, IEnumerable<SubRows> parentRows)
        {
            if (_fields.Length == 0)
            {
                var g = new RowGroup();
                foreach (var parentRow in parentRows)
                    g.AddRow((TablRow)parentRow, false);
                return new SubRows[] { g };
            }
            var dic = new GroupDic();
            foreach (var parentRow in parentRows)
            {
                 
            }
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //Вспомогательный класс - группировка по одному полю
    internal class GroupDic
    {
        public GroupDic(string fieldName, Mean fieldMean)
        {
            _fieldName = fieldName;
            _fieldMean = fieldMean;
        }

        //Имя и значение поля, по которым идет группировка
        private string _fieldName;
        private Mean _fieldMean;
        
        //Словарь с ключами - значениями поля группировки и значениемя - словарями группировки по следующим полям
        private DicS<GroupDic> _dic;
        public DicS<GroupDic> Dic { get { return _dic ?? (_dic = new DicS<GroupDic>()); }}
        //Список рядов, для словаря по последнему полю
        private List<TablRow> _rows;
        public List<TablRow> Rows { get { return _rows ?? (_rows = new List<TablRow>()); } }

        //Добавляет ряд в структуру группировки
        public void AddRow(TablRow row, string[] fields, int level)
        {
            string s = row[fields[level]].String ?? "";
            if (level < fields.Length)
                Dic.Add(s, new GroupDic(fields[level], row[fields[level]]))
                         .AddRow(row, fields, level + 1);
            else Rows.Add(row);
        }

        //Итоговый список групп
        public IEnumerable<RowGroup> GetGroups()
        {
            if (_rows != null)
            {
                var g = new RowGroup();
                foreach (var tablRow in Rows)
                    g.AddRow(tablRow, false);
                return new [] { g };
            }
            foreach (var pair in Dic.Dic)
                pair.Value.
                   
        }
    }
}