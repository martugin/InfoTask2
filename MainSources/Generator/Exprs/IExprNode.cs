namespace CommonTypes
{
    //Интерфейс для узлов, у которых может быть вычислено значение
    internal interface IExprNode : INode
    {
        //Проверка корректности выражений генерации, определение типа данных выражения
        DataType Check(ITablStruct tabl);
        //Вычисление значения по ряду исходной таблицы
        IReadMean Generate(SubRows row);
    }

    //------------------------------------------------------------------------------------------------------
    //Интерфейс для узлов без значения
    internal interface IVoidNode : INode
    {
        //Проверка корректности выражений генерации
        void Check(ITablStruct tabl);
        //Выполнение действий по ряду исходной таблицы
        void Generate(SubRows row);
    }
}