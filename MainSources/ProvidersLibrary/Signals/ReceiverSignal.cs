using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(IReceiver receiver, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
        }

        //Передаваемое значение
        public IMean Value { get; set; }
    }
}