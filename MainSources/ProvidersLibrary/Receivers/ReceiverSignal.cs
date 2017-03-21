using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(ReceiverConnect connect, string code, DataType dataType, string infObject, string infOut, string infProp)
            : base(code, dataType, infObject, infOut, infProp)
        {
            Connect = connect;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected ReceiverConnect Connect { get; private set; }
    }
}