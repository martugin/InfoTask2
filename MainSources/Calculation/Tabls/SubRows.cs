using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Интерфейс для строк таблиц и данных старшей таблицы
    public class SubRows
    {
        //Список строк подчиненных таблиц
        private List<TablRow> _subList;
        public List<TablRow> SubList { get { return _subList ?? (_subList = new List<TablRow>()); }}
        //Словари строк подчиненных таблиц по полям Num и Code
        private DicI<TablRow> _subNums;
        public DicI<TablRow> SubNums { get { return _subNums ?? (_subNums = new DicI<TablRow>()); } }
        private DicS<TablRow> _subCodes;
        public DicS<TablRow> SubCodes { get { return _subCodes ?? (_subCodes = new DicS<TablRow>()); } }
        //Ряд - родитель или null, если это данные старшей таблицы
        public SubRows Parent { get; set; }

        //Добавить строку подтаблицы
        public void AddRow(TablRow row,
                                      bool addIndices) //Добавлять индексирование по полям Code и Num
        {
            row.Parent = this;
            SubList.Add(row);
            if (addIndices)
            {
                SubNums.Add(row.Num, row);
                if (!row.Code.IsEmpty())
                    SubCodes.Add(row.Code, row);    
            }
        }

        //Словарь значений, ключи - коды полей
        private readonly DicS<Mean> _means = new DicS<Mean>();
        public DicS<Mean> Means { get { return _means; } }

        //Доступ к значению по имени поля 
        public Mean this[string name]
        {
            get { return _means.ContainsKey(name) ? _means[name] : null; }
        }
    }
}