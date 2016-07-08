using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(Receiver receiver, DataType dataType, string signalInf)
            : base(dataType, signalInf)
        {
            Receiver = receiver;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected Receiver Receiver { get; private set; }
    }
}