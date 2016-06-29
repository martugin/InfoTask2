namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class Receiver : ProviderBase
    {
        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }


    }
}