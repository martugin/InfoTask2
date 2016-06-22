using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Стандартный интерфейс для класса Receiver
    public interface IReceiver : IProvider
    {
        //Список сигналов приемника с мгновенными значениями
        IDicSForRead<ReceiverSignal> Signals { get; }
        //Добавить сигнал приемника
        ReceiverSignal AddSignal(string signalInf, //Информация о сигнале для приемника
                                               string code, //Код сигнала
                                               DataType dataType); //Тип данных
        //Отправить значения параметров на приемник
        void WriteValues();
        //Допускается передача списка мгновенных значений за один раз
        bool AllowListValues { get; }
    }
}