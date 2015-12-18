using BaseLibrary;

namespace CommonTypes
{
    //Одна строка таблицы
    public class TablRow : SubRows
    {
        public TablRow(Tabl tabl, int level, IRecordRead rec)
        {
            Code = rec.GetString("Code");
            Num = rec.GetInt("Num");
            Id = rec.GetInt("Id");
            if (level > 0) ParentId = rec.GetInt("ParentId");
            _fieldsDic = tabl.Fields[level];
            _fields = new Mean[_fieldsDic.Count];
            foreach (var f in _fieldsDic.Values)
                _fields[f.Num] = rec.GetMean(f.DataType, f.Name);
        }

        //Id себя и родителя
        public int Id { get; private set; }
        public int ParentId { get; private set; }
        //Ключи числовой и строковый
        public string Code { get; private set; }
        public int Num { get; private set; }

        //Словарь полей 
        private readonly DicS<TablField> _fieldsDic;
        //Массив значений полей
        private readonly Mean[] _fields;

        //Проверка на наличие полей в строке
        public bool Contains(int num)
        {
            return num >= 0 || num < _fields.Length;
        }
        public bool Contains(string name)
        {
            return _fieldsDic.ContainsKey(name);
        }

        //Доступ к значению по номеру поля 
        public Mean this[int num]
        {
            get
            {
                if (Contains(num))
                    return _fields[num];
                return null;
            }
        }
        //Доступ к значению по имени поля 
        public Mean this[string name]
        {
            get
            {
                if (Contains(name))
                    return _fields[_fieldsDic[name].Num];
                return null;
            }
        }
    }
}