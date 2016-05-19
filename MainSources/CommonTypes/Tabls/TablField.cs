namespace CommonTypes
{
    //Поле таблицы
    public class TablField
    {
        public TablField(string name, int num, DataType dataType)
        {
            Name = name;
            Num = num;
            DataType = dataType;
        }

        //Имя поля
        public string Name { get; private set; }
        //Номер поля
        public int Num { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }

        //Сравнение по всем характеристикам
        public bool IsEquals(TablField field)
        {
            return Num == field.Num && Name == field.Name && DataType == field.DataType;
        }
    }
}