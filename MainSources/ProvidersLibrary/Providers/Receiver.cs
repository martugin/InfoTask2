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

        //Подготовка выходов
        protected override void PrepareOuts()
        {
            foreach (var sig in ReceiverConnect.Signals.Values)
                if (sig.Type != SignalType.Calc)
                {
                    var ob = AddOut(sig);
                    ob.Context = sig.ContextOut;
                    ob.AddSignal(sig);
                }
        }

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}