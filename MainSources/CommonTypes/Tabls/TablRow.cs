using BaseLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace CommonTypes
{
    //Базовый класс для ряда таблицы и сгруппированных рядов таблицы
    public class RowGroup : SubRows
    {
        //Словарь значений, ключи - коды полей
        private readonly DicS<Mean> _means = new DicS<Mean>();
        public DicS<Mean> Means { get { return _means; } }

        //Доступ к значению по имени поля 
        public Mean this[string name]
        {
            get
            {
                if (_means.ContainsKey(name))
                    return _means[name];
                return null;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------

    //Одна строка таблицы
    public class TablRow : RowGroup
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