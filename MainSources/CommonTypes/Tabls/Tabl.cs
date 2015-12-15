using System;
using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Поле таблицы
    public class TablField
    {
        public TablField(string name, int num, DataType fataType)
        {
            Name = name;
            Num = num;
            DataType = fataType;
        }

        //Имя поля
        public string Name { get; private set; }
        //Номер поля
        public int Num { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }

        //Сравнение по всем характеристикам
        public bool IsEquals(TablField field)
        {
            return Num == field.Num && Name == field.Name && DataType == field.DataType;
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Таблица со всеми вложенными
    public class Tabl
    {
        //Проверка, что таблица базы Access является таблицей
        //Возвращает пару <код, уровень вложенности>, или null, если не является нужной таблицей
        public static Tuple<string, int> GetTabl(string tname)
        {
            if (!tname.StartsWith("Tbl_") || tname.Length < 10 || tname.Substring(tname.Length - 5, 4) != "_Sub")
                return null;
            string code = tname.Substring(4, tname.Length - 9);
            int level = tname.Substring(tname.Length - 1, 1).ToInt();
            return new Tuple<string, int>(code, level);
        }

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
        //И сразу загрузить список полей из таблицы в Fields
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
            return "Tbl_" + Code + "_Sub" + level;
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