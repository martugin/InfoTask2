using BaseLibrary;

namespace CommonTypes
{
    //Данные старшей таблицы
    public class Tabl : SubRows
    {
        public Tabl(int maxLevel)
        {
            _rows = new DicI<TablRow>[maxLevel - 1];
            for (int i = 0; i < maxLevel; i++)
                _rows[i] = new DicI<TablRow>();
        }

        //Массив словарей рядов таблиц разной вложенности
        //Индекс массива - уровень таблицы, ключи словарей - значения полей Id
        private readonly DicI<TablRow>[] _rows;
        public DicI<TablRow>[] Rows { get { return _rows; } }

        //Добавляет ряд значений на указанный уровень
        public void AddRow(int level, TablRow row)
        {
            if (level < Rows.Length - 1)
                Rows[level].Add(row.Id, row);
            var parent = level == 0 ? (SubRows)this : Rows[level - 1][row.ParentId];
            parent.AddRow(row);
        }
    }
}