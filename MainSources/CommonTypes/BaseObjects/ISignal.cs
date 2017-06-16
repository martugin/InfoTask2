namespace CommonTypes
{
    //Базовый интерфейс для сигналов и параметров
    public interface ISignal
    {
        //Полный код
        string Code { get; }
        //Тип данных значения
        DataType DataType { get; }
    }

    //-----------------------------------------------------------------------------
    //Интерфейс для сигналов и параметров для чтения
    public interface IReadSignal : ISignal
    {
        //Возвращаемое значение
        IReadMean OutValue { get; }
    }

    //-----------------------------------------------------------------------------
    //Интерфейс для сигналов и параметров для записи
    public interface IWriteSignal : ISignal
    {
        //Исходный сигнал
        IReadSignal InSignal { get; }
    }
}