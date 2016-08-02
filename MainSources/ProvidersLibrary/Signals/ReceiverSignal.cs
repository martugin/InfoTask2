using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(ReceiverConnect connect, string code, string codeObject, DataType dataType, string signalInf)
            : base(code, codeObject, dataType, signalInf)
        {
            ReceiverConnect = connect;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected ReceiverConnect ReceiverConnect { get; private set; }
    }
}