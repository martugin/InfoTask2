using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Одна строка таблицы
    public class TablRow : SubRows
    {
        public TablRow(RecDao rec)
        {
            foreach (Field f in rec.Recordset.Fields)
                Means.Add(f.Name, rec.GetMean(f.Type.ToDataType(), f.Name));
        }
        
        //Id себя и родителя
        public int Id { get { return Means["Id"].Integer; } }
        public int ParentId { get { return Means["ParentId"].Integer; } }
        //Ключи числовой и строковый
        public string Code { get { return Means.ContainsKey("Code") ? Means["Code"].String : null; } }
        public int Num { get { return Means.ContainsKey("Num") ? Means["Num"].Integer : 0; } }
    }
}