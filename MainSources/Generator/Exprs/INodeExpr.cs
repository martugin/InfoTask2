namespace CommonTypes
{
    //Интерфейс для узлов, у которых может быть вычислено значение
    internal interface INodeExpr : INode
    {
        //Проверка корректности выражений генерации
        DataType Check(TablStructItem row);
        //Вычисление значения по ряду исходной таблицы
        Mean Process(SubRows row);
    }

    //------------------------------------------------------------------------------------------------------
    //Интерфейс для узлов без значения
    internal interface INodeVoid : INode
    {
        //Проверка корректности выражений генерации
        void Check(TablStructItem row);
        //Выполнение действий по ряду исходной таблицы
        void Process(SubRows row);
    }
}