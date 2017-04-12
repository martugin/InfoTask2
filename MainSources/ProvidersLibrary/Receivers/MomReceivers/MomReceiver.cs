namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class MomReceiver : Provider
    {
        //Ссылка на соединение
        internal MomReceiverConnect ReceiverConnect
        {
            get { return (MomReceiverConnect)ProviderConnect; }
        }

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}