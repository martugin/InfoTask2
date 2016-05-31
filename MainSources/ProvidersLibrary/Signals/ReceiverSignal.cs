namespace CommonTypes
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(string signalInf, string code, DataType dataType, IReceiver provider) 
            :base(signalInf, code, dataType)
        {
        }

        //Передаваемое значение
        public IMean Value { get; set; }
    }
}