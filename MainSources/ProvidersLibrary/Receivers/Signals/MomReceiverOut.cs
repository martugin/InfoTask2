namespace ProvidersLibrary
{
    public abstract class MomReceiverOut : ProviderOut 
    {
        protected MomReceiverOut(MomReceiver receiver) 
            : base(receiver) { }

        //Ссылка на приемник
        protected MomReceiver Receiver { get { return (MomReceiver) Provider; } }

        //Основной сигнал объекта
        protected internal ProviderSignal ValueSignal { get; set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}