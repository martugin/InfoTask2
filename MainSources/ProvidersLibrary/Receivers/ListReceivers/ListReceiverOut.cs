namespace ProvidersLibrary
{
    public abstract class ListReceiverOut : ProviderOut 
    {
        protected ListReceiverOut(MomReceiver receiver) 
            : base(receiver) { }

        //Ссылка на приемник
        protected ListReceiver Receiver { get { return (ListReceiver) Provider; } }

        //Основной сигнал объекта
        protected ListSignal ValueSignal { get; set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return ValueSignal = ValueSignal ?? (ListSignal)sig;
        }
        protected virtual ListSignal AddListSignal(ListSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}