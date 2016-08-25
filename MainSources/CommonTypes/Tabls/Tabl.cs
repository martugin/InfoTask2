using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Таблица со всеми вложенными
    public class Tabl
    {
        //Создание таблицы
        public Tabl(string code, //код таблицы
                          int level, // уровень вложенности
                          DaoDb db) //База данных
        {
            Code = code;
            _db = db;
            AddLevel(level);
        }

        //Задать максимальный уровень
        //И сразу загрузить список полей из таблицы в _fields
        public void AddLevel(int level)
        {
            var tname = TablName(level);
            var dicn = new DicI<TablField>();
            var dic = new DicS<TablField>();
            var t = _db.Database.TableDefs[tname];
            int i = 0;
            foreach (Field f in t.Fields)
                if (f.Name != "Id" && f.Name != "ParentId" && f.Name != "Num" && f.Name != "Code")
                {
                    var tf = new TablField(f.Name, i, f.Type.ToDataType());
                    dic.Add(f.Name, tf);
                    dicn.Add(i++, tf);
                }
            Fields.Add(level, dic);
            FieldsNums.Add(level, dicn);
        }

        //База данных таблиц
        private readonly DaoDb _db;
        //Код
        public string Code { get; private set; }
        //Максимальный уровень вложенности
        public int MaxLevel { get { return Fields.Count - 1; } }

        //Имя таблицы указанной вложенности
        private string TablName(int level)
        {
            if (level == 0) return Code;
            return Code + "_Sub" + level;
        }

        //Словари номеров полей с ключами - названиями, для разных подтаблиц
        private readonly DicI<DicS<TablField>> _fields = new DicI<DicS<TablField>>();
        public DicI<DicS<TablField>> Fields { get { return _fields; } }
        //Словари типов данных полей с ключами - номерами, для разных подтаблиц
        private readonly DicI<DicI<TablField>> _fieldsNums = new DicI<DicI<TablField>>();
        public DicI<DicI<TablField>> FieldsNums { get { return _fieldsNums; }}

        //Данные старшей таблицы
        public TablValues TablValues { get; private set; }

        //Загрузить данные подтаблиц всех уровней
        public void LoadValues()
        {
            TablValues = new TablValues(MaxLevel);
            for (int i = 0; i <= MaxLevel; i++)
                using (var rec = new RecDao(_db, "SELECT * FROM " + TablName(i)))
                    while (rec.Read())
                    {
                        var row = new TablRow(this, i, rec);
                        if (i < MaxLevel)
                            TablValues.Rows[i].Add(row.Id, row);
                        var parent = i == 0 ? (SubRows)TablValues : TablValues.Rows[i - 1][row.ParentId];
                        parent.AddRow(row);
                        row.Parent = parent;
                    }
        }
    }
}