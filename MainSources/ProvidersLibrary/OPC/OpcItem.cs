using CommonTypes;

namespace ProvidersLibrary
{
    //Один параметр для записи в OPC
    public class OpcItem : ReceiverSignal
    {
        internal OpcItem(Receiver receiver, string code, DataType dataType, string signalInf)
            : base(receiver, code, dataType, signalInf)
        {}

        //Стандартные свойства
        public string Tag { get; set; }
        public int ClientHandler { get; set; }
        public int ServerHandler { get; internal set; }

        //Значение
        public IMean Mom { get { return Value; } }
    }
}