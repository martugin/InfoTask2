using BaseLibrary;

namespace ProvidersLibrary
{
    public class ListReceiverConnect : ReceiverConnect
    {
        public ListReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Текущий провайдер источника
        internal ListReceiver Receiver { get { return (ListReceiver)Provider; } }

        //Записать значения
        protected override void PutValues()
        {
            throw new System.NotImplementedException();
        }
    }
}