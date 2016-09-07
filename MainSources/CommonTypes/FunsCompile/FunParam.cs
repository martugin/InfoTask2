namespace CommonTypes
{
    //Один параметр функции
    internal class FunParam
    {
        internal FunParam(string dtype, string def = null)
        {
            DataType = dtype.ToDataType();
            if (DataType == DataType.Error)
                DataType = DataType.Variant;
            Default = def;
        }

        //Тип данных, встроенный тип или вариант для других случаев
        internal DataType DataType { get; private set; }
        //Значение по умолчанию или null
        internal string Default { get; private set; }
        //True, если участвует в формировании типа результата
        internal bool UsedInResult { get; set; }
    }
}