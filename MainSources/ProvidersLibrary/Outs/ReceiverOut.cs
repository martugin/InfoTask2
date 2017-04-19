namespace ProvidersLibrary
{
    public abstract class ReceiverOut : ProviderOut 
    {
        protected ReceiverOut(Receiver receiver) 
            : base(receiver) { }

        //Ссылка на приемник
        protected Receiver Receiver
        {
            get { return (Receiver) Provider; }
        }

        //Основной сигнал объекта
        protected internal ReceiverSignal ValueSignal { get; set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return AddReceiverSignal((ReceiverSignal)sig);
        }
        protected virtual ReceiverSignal AddReceiverSignal(ReceiverSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}