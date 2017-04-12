using BaseLibrary;

namespace ProvidersLibrary
{
    public abstract class ListReceiverConnect : ReceiverConnect
    {
        protected ListReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Текущий провайдер источника
        internal ListReceiver Receiver { get { return (ListReceiver)Provider; } }
    }
}