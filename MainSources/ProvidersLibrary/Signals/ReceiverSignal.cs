using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProvSignal
    {
        public ReceiverSignal(IReceiver receiver, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
        }

        //Передаваемое значение
        public IMean Value { get; set; }
    }
}