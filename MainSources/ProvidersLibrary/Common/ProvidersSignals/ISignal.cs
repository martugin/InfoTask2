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
        //Тип значений сигнала
        SignalValueType ValueType { get; }

        //Значение (одно или список мгновенных значений)
        IMean Value { get; }
    }
}