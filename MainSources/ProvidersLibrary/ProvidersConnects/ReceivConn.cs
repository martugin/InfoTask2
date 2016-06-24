namespace ProvidersLibrary
{
    //Соединение - приемник
    public abstract class ReceivConn : ProvConn
    {
        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }

        //Текущий провайдер-приемник
        private Receiv _receiver;
        public Receiv Receiver
        {
            get
            {
                if (_receiver == null && MainProvider != null)
                    _receiver = (Receiv)MainProvider;
                return _receiver;
            }
        }
    }
}