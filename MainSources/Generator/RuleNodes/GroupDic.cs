using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Вспомогательный класс - группировка по одному полю
    internal class GroupDic
    {
        public GroupDic(string fieldName, Mean fieldMean)
        {
            _fieldName = fieldName;
            _fieldMean = fieldMean;
        }
        public GroupDic() { } //Старший уровень

        //Имя и значение поля, по которым идет группировка
        private readonly string _fieldName;
        private readonly Mean _fieldMean;
        
        //Словарь с ключами - значениями поля группировки и значением - словарями группировки по следующим полям
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
        public IEnumerable<SubRows> GetGroups()
        {
            if (_rows != null)
            {
                var g = new SubRows();
                foreach (var tablRow in Rows)
                    g.AddRow(tablRow, false);
                return new [] { g };
            }
            var glist = new List<SubRows>();
            foreach (var grDic in Dic.Dic.Values)
            {
                var groups = grDic.GetGroups();
                foreach (var rowGroup in groups)
                {
                    rowGroup.Means.Add(_fieldName ,_fieldMean);
                    glist.Add(rowGroup);
                }
            }
            return glist;
        }
    }
}