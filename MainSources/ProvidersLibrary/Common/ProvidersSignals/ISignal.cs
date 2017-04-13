using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Интерфейс для всех сигналов
    public interface ISignal
    {
        //Код
        string Code { get; }
        //Тип данных
        DataType DataType { get; }
        //Настройки сигнала
        DicS<string> Inf { get; }
        //Настройки выхода отдельно
        string ContextOut { get; }
        //Соединение
        ProviderConnect Connect { get; }
        //Является основным сигналом (не расчетным и т.п.)
        bool IsInitial { get; }
    }

    //------------------------------------------------------------------------------------
    //Интерфейс для сигналов источника
    public interface ISourceSignal : ISignal
    {
        //Значение (одно или список мгновенных значений)
        IMean Value { get; }
    }

    //------------------------------------------------------------------------------------
    //Интерфейс для сигналов приемника
    public interface IReceiverSignal : ISignal
    {
        //Присвоить значение для записи в приемник
        IMean Value { set; }
    }
}