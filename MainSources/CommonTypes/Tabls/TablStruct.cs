using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для структур таблиц и сгруппированых рядов
    public interface IRowStruct
    {
        //Словарь полей, ключи - коды, значения - типы данных
        DicS<DataType> Fields { get; }
    }

    //-----------------------------------------------------------------------------------------------------------------------------

    //Структура одной пользовательской таблицы
    public class TablStruct : IRowStruct
    {
        public TablStruct(string name, int level)
        {
            TableName = name;
            Level = level;
        }

        //Имя таблицы
        public string TableName { get; private set; }
        //Уровень таблицы в группе
        public int Level { get; private set; }
        //Слудующий и предыдущий уровни таблицы
        public TablStruct Child { get; internal set; }
        public TablStruct Parent { get; internal set; }

        //Словарь полей, ключи - коды, значения - типы данных
        private readonly DicS<DataType> _fields = new DicS<DataType>();
        public DicS<DataType> Fields  { get { return _fields; } }
    }
}