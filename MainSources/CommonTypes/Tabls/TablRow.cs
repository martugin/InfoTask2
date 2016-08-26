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
        public string Code { get { return _means["Code"].String; } }
        public int Num { get { return _means["Num"].Integer; } }
        //Тип
        public string Type { get { return _means["Type"].String; } }

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