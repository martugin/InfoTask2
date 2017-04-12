namespace ProvidersLibrary
{
    //Один итем для записи в OPC
    public class OpcItem : MomReceiverOut
    {
        internal OpcItem(MomReceiver receiver, string tag, int clientHandler)
            : base(receiver)
        {
            Tag = tag;
            ClientHandler = clientHandler;
        }
        
        //Стандартные свойства
        public string Tag { get; private set; }
        internal int ClientHandler { get; private set; }
        internal int ServerHandler { get; set; }
    }
}