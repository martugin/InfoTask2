using BaseLibrary;

namespace ProvidersLibrary
{
    //Соединение - приемник
    public class ReceiverConnect : ProviderConnect
    {
        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }

        //Текущий провайдер источника
        public ReceiverBase Curreceiver { get { return (ReceiverBase)CurProvider; } }

        //Словарь сигналов приемников, ключи - коды
        protected readonly DicS<ReceiverSignal> ProviderSignals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return ProviderSignals; } }
    }
}