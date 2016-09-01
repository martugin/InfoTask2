using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Структура групы пользовательских таблиц
    public class TablSruct
    {
        public TablSruct(string dbFile, string code)
        {
            DbFile = dbFile;
            Code = code;
            Tabls.Add(-1, new TablStructItem("", -1)); //Искусственный родитель главной таблицы
        }

        //Код группы таблиц или имя главной таблицы
        public string Code { get; private set; }
        //Имя базы данных
        public string DbFile { get; private set; }
        //Словарь таблиц, ключ - уровень таблицы
        private readonly DicI<TablStructItem> _tabls = new DicI<TablStructItem>();
        public DicI<TablStructItem> Tabls { get { return _tabls; } }

        //Добавить таблицу в на один из уровней структуры
        public void AddTabl(DaoDb db, //База данных
                                       string tableName, //Имя таблицы
                                       int level) //Уровень таблицы в группе
        {
            var tsi = new TablStructItem(tableName, level);
            Tabls.Add(level, tsi);
            var t = db.Database.TableDefs[tableName];
            foreach (Field f in t.Fields)
                tsi.Fields.Add(f.Name, f.Type.ToDataType());
            if (Tabls.ContainsKey(level - 1))
            {
                tsi.Parent = Tabls[level - 1];
                Tabls[level - 1].Child = tsi;
            }
            if (Tabls.ContainsKey(level + 1))
            {
                tsi.Child = Tabls[level + 1];
                Tabls[level + 1].Parent = tsi;
            }
        }
    }
}