using CommonTypes;

namespace CompileLibrary
{
    //Один параметр функции
    internal class FunParam
    {
        internal FunParam(string dtype, string atype = "", string def = "")
        {
            DataType = dtype.ToDataType();
            if (DataType == DataType.Error)
                DataType = DataType.Variant;
            ArrayType = atype.ToArrayType();
            Default = def;
        }

        //Тип данных, встроенный тип или вариант для других случаев
        internal DataType DataType { get; private set; }
        //Тип массива
        internal ArrayType ArrayType { get; private set; }
        //Значение по умолчанию или null
        internal string Default { get; private set; }
        //True, если участвует в формировании типа результата
        internal bool UsedInResult { get; set; }
    }
}