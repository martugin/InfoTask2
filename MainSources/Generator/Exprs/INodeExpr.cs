namespace CommonTypes
{
    //Интерфейс для узлов, у которых может быть вычислено значение
    internal interface INodeExpr : INode
    {
        //Проверка корректности выражений генерации, определение типа данных выражения
        DataType Check(TablStruct tabl);
        //Вычисление значения по ряду исходной таблицы
        Mean Generate(SubRows row);
    }

    //------------------------------------------------------------------------------------------------------
    //Интерфейс для узлов без значения
    internal interface INodeVoid : INode
    {
        //Проверка корректности выражений генерации
        void Check(TablStruct tabl);
        //Выполнение действий по ряду исходной таблицы
        void Generate(SubRows row);
    }
}