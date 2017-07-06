using CommonTypes;

namespace CompileLibrary
{
    //Перегрузка, подходящая под выражение
    public class FunSelected
    {
        public FunSelected(FunOverload overload, DataType dataType, ArrayType arrayType)
        {
            Overload = overload;
            DataType = dataType;
            ArrayType = arrayType;
        }

        //Перегрузка
        public FunOverload Overload { get; private set; }
        //Возвращаемый тип данных
        public DataType DataType { get; private set; }
        //Возвращаемый тип массива
        public ArrayType ArrayType { get; private set; }
    }
}