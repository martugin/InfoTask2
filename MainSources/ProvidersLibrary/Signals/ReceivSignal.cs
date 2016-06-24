using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceivSignal : ProvSignal
    {
        public ReceivSignal(ReceivConn conn, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
            ReceivConn = conn;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected ReceivConn ReceivConn { get; private set; }
    }
}