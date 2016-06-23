namespace ProvidersLibrary
{
    //Соединение - приемник
    public abstract class ReceivConn : ProvConn
    {
        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }

        //Текущий провайдер-приемник
        private ReceivBase _receiver;
        public ReceivBase Receiver
        {
            get
            {
                if (_receiver == null && MainProvider != null)
                    _receiver = (ReceivBase)MainProvider;
                return _receiver;
            }
        }
    }
}