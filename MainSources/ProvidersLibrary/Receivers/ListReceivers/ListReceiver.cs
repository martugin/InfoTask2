namespace ProvidersLibrary
{
    public abstract class ListReceiver : Provider
    {
        //Ссылка на соединение
        internal ListReceiverConnect ReceiverConnect
        {
            get { return (ListReceiverConnect)ProviderConnect; }
        }

    }
}