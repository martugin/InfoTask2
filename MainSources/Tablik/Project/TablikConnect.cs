using CommonTypes;

namespace Tablik
{
    //Проайдер для компилятора
    public abstract class TablikConnect : BaseConnect
    {
        protected TablikConnect(BaseProject project, string code, string complect) 
            : base(project, code, complect) { }
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    public class TablikSource : TablikConnect
    {
        protected TablikSource(BaseProject project, string code, string complect)
            : base(project, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    public class TablikReceiver : TablikConnect
    {
        protected TablikReceiver(BaseProject project, string code, string complect)
            : base(project, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
    }
}