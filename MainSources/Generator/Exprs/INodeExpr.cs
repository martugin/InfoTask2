namespace CommonTypes
{
    //Интерфейс для узлов, у которых может быть вычислено значение
    internal interface INodeExpr : INode
    {
        //Вычисление значения по ряду исходной таблицы
        Mean Process(TablRow row);
    }

    //------------------------------------------------------------------------------------------------------
    //Интерфейс для узлов без значения
    internal interface INodeVoid : INode
    {
        //Выполнение действий по ряду исходной таблицы
        void Process(TablRow row);
    }
}