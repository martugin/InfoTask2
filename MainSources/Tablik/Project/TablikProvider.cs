using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Проайдер для компилятора
    public abstract class TablikProvider : BaseConnect
    {
        protected TablikProvider(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    public class TablikSource : TablikProvider
    {
        protected TablikSource(string name, string complect, Logger logger)
            : base(name, complect, logger) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    public class TablikReceiver : TablikProvider
    {
        protected TablikReceiver(string name, string complect, Logger logger)
            : base(name, complect, logger) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
    }
}