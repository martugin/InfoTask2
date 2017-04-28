using BaseLibrary;

namespace CommonTypes
{
    public interface IConnect
    {
        //Код соединения
        string Code { get; }
    }

    //----------------------------------------------------------------------------------------
    //Соединение для чтения данных
    public interface IReadConnect : IConnect
    {
        //Список сигналов для чтения значений
        IDicSForRead<IReadSignal> ReadingSignals { get; }
    }

    //-----------------------------------------------------------------------------------------
    //Соединение для чтения данных
    public interface IWriteConnect : IConnect
    {
        //Список сигналов для записи значений
        IDicSForRead<IWriteSignal> WritingSignals { get; }
    }
}