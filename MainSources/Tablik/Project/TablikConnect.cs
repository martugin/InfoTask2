using CommonTypes;

namespace Tablik
{
    //Проайдер для компилятора
    public abstract class TablikConnect : BaseConnect
    {
        protected TablikConnect(TablikProject tablik, string code, string complect) 
            : base(tablik.Project.App, code, complect, tablik.Project.Code) { }
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    public class TablikSource : TablikConnect
    {
        protected TablikSource(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    public class TablikReceiver : TablikConnect
    {
        protected TablikReceiver(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
    }
}