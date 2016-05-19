using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для строк таблиц и данных старшей таблицы
    public abstract class SubRows
    {
        //Словари строк подчиненных таблиц по полям Num и Code
        private readonly DicI<TablRow> _subNums = new DicI<TablRow>();
        public DicI<TablRow> SubNums { get { return _subNums; } }
        private readonly DicS<TablRow> _subCodes = new DicS<TablRow>();
        public DicS<TablRow> SubCodes { get { return _subCodes; } }
        //Список строк подчиненных таблиц
        private readonly List<TablRow> _subs = new List<TablRow>();
        public List<TablRow> Subs { get { return _subs; } }
        //Ряд - родитель или null, если это данные старшей таблицы
        public SubRows Parent { get; set; }

        //Добавить строку подтаблицы
        public void AddRow(TablRow row)
        {
            SubNums.Add(row.Num, row);
            if (!row.Code.IsEmpty())
                SubCodes.Add(row.Code, row);
            Subs.Add(row);
        }
    }
    
    //----------------------------------------------------------------------------------------------------------
    //Данные старшей таблицы
    public class TablValues : SubRows
    {
        public TablValues(int maxLevel)
        {
            _rows = new DicI<TablRow>[maxLevel];
            for (int i = 0; i < maxLevel; i++)
                _rows[i] = new DicI<TablRow>();
        }

        //Массив словарей рядов таблиц разной вложенности
        //Индекс массива - уровень таблицы, ключи словарей - значения полей Id
        private readonly DicI<TablRow>[] _rows;
        public DicI<TablRow>[] Rows { get { return _rows; } }
    }
}