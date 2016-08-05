namespace ProvidersLibrary
{
    //Один параметр для записи в OPC
    public class OpcItem : ReceiverObject
    {
        internal OpcItem(ReceiverBase receiver, string tag, int clientHandler)
            : base(receiver)
        {
            Tag = tag;
            ClientHandler = clientHandler;
        }
        
        //Стандартные свойства
        public string Tag { get; private set; }
        public int ClientHandler { get; private set; }
        public int ServerHandler { get; internal set; }
    }
}