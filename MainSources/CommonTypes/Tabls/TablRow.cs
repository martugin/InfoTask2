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
                _means.Add(f.Name, rec.GetMean(f.Type.ToDataType(), f.Name));
        }
        
        //Id себя и родителя
        public int Id { get { return _means["Id"].Integer; } }
        public int ParentId { get { return _means["ParentId"].Integer; } }
        //Ключи числовой и строковый
        public string Code { get { return _means.ContainsKey("Code") ? _means["Code"].String : null; } }
        public int Num { get { return _means.ContainsKey("Num") ?_means["Num"].Integer : 0; } }
        //Тип
        public string Type { get { return _means.ContainsKey("GenType") ?  _means["GenType"].String : null; } }

        //Словарь значений, ключи - коды полей
        private readonly DicS<Mean> _means = new DicS<Mean>();
        
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
}