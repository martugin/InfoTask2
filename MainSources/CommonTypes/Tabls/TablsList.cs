using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Список таблиц одного файла
    public class TablsList
    {
        //Словарь структур таблиц
        private readonly DicS<TablGroup> _structs = new DicS<TablGroup>();
        public DicS<TablGroup> Structs { get { return _structs; } }
        //Словарь значений таблиц
        private readonly DicS<Tabl> _tabls = new DicS<Tabl>();
        public DicS<Tabl> Tabls { get { return _tabls; } }

        //Добавляет структуру одной группы таблиц
        public TablGroup AddStruct(DaoDb db, //База данных
                                        string table0, string table1 = null, string table2 = null, string table3 = null) //Имена таблиц разной вложенности
        {
            var s = new TablGroup(db.File, table0);
            Structs.Add(table0, s);
            s.AddTabl(db, table0, 0);
            if (!table1.IsEmpty()) s.AddTabl(db, table1, 1);
            if (!table2.IsEmpty()) s.AddTabl(db, table2, 2);
            if (!table3.IsEmpty()) s.AddTabl(db, table3, 3);
            return s;
        }

        //Добавляет структуры всех таблиз базы фомата Tbl_*_sub*
        public void AddDbStructs(DaoDb db) //База данных
        {
            db.ConnectDao();
            foreach (TableDef t in db.Database.TableDefs)
                if (t.Name.StartsWith("Tbl_") && t.Name.Substring(t.Name.Length - 5, 4) == "_Sub")
                {
                    int level = t.Name.Substring(t.Name.Length - 1).ToInt();
                    string code = t.Name.Substring(4, t.Name.Length - 9);
                    if (!Structs.ContainsKey(code))
                        Structs.Add(code, new TablGroup(db.File, code));
                    Structs[code].AddTabl(db, t.Name, level);
                }
        }

        //Загрузка значений всех таблиц одной базы
        public void LoadValues(DaoDb db)
        {
            foreach (var s in Structs.Dic)
                if (s.Value.DbFile == db.File)
                {
                    var tabl = Tabls.Add(s.Key, new Tabl(s.Value.Tabls.Count));
                    foreach (var tsi in s.Value.Tabls.Values)
                        using (var rec = new RecDao(db, tsi.TableName))
                            while (rec.Read())
                                tabl.AddRow(new TablRow(rec));
                }
        }
    }
}