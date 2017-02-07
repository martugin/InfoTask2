using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(ReceiverConnect connect, string code, string codeOut, DataType dataType, string signalInf)
            : base(code, codeOut, dataType, signalInf)
        {
            Connect = connect;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected ReceiverConnect Connect { get; private set; }
    }
}