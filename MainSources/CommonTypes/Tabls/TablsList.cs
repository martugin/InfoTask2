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
            {
                var tup = Tabl.GetTabl(t.Name);
                if (tup != null)
                {
                    if (Tabls.ContainsKey(tup.Item1))
                        Tabls[tup.Item1].AddLevel(tup.Item2);
                    else Tabls.Add(tup.Item1, new Tabl(tup.Item1, tup.Item2, db));
                }
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