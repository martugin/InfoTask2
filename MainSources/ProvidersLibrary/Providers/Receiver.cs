namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class Receiver : Provider
    {
        //Ссылка на соединение
        internal ReceiverConnect ReceiverConnect
        {
            get { return (ReceiverConnect)ProviderConnect; }
        }

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}