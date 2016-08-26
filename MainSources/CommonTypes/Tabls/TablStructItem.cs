using BaseLibrary;

namespace CommonTypes
{
    //Структура одной пользовательской таблицы
    public class TablStructItem
    {
        public TablStructItem(string name, int level)
        {
            TableName = name;
            Level = level;
        }

        //Имя таблицы
        public string TableName { get; private set; }
        //Уровень таблицы в группе
        public int Level { get; private set; }

        //Словарь полей, ключи - коды, значения - типы данных
        private readonly DicS<DataType> _fields = new DicS<DataType>();
        public DicS<DataType> Fields  { get { return _fields; } }
    }
}