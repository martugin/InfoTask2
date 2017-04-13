namespace ProvidersLibrary
{
    public abstract class ListReceiverOut : ProviderOut 
    {
        protected ListReceiverOut(ListReceiver receiver) 
            : base(receiver) { }

        //Ссылка на приемник
        protected ListReceiver Receiver { get { return (ListReceiver) Provider; } }

        //Основной сигнал объекта
        protected internal ListSignal ValueSignal { get; set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return AddListSignal((ListSignal) sig);
        }
        protected virtual ListSignal AddListSignal(ListSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}