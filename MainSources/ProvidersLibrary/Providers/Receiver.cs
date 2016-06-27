namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class Receiver : Provider
    {
        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }


    }
}