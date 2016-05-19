using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Список таблиц одного файла
    public class TablsList
    {
        //Загрузка всех таблиц для указанной базы данных
        public TablsList(DaoDb db)
        {
            db.ConnectDao();
            foreach (TableDef t in db.Database.TableDefs)
                if (t.Name != "SysTabl" && t.Name != "SysSubTabl")
                {
                    int level = 0;
                    string tname = t.Name;
                    if (t.Name.Substring(t.Name.Length - 5, 4) == "_Sub")
                    {
                        level = t.Name.Substring(t.Name.Length - 1).ToInt();
                        tname = t.Name.Substring(0, t.Name.Length - 5);
                    }
                    if (Tabls.ContainsKey(tname))
                        Tabls[tname].AddLevel(level);
                    else Tabls.Add(tname, new Tabl(tname, level, db));
                }
        }

        //Словарь таблиц
        private readonly DicS<Tabl> _tabls = new DicS<Tabl>();
        public DicS<Tabl> Tabls { get { return _tabls; } }

        //Загрузка значений всех таблиц списка
        public void LoadValues()
        {
            foreach (var tabl in Tabls.Values)
                tabl.LoadValues();
        }
    }
}