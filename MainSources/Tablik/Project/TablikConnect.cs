using CommonTypes;

namespace Tablik
{
    //Проайдер для компилятора
    public abstract class TablikConnect : BaseConnect
    {
        protected TablikConnect(Project project, string name, string complect) 
            : base(project, name, complect) { }
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    public class TablikSource : TablikConnect
    {
        protected TablikSource(Project project, string name, string complect)
            : base(project, name, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    public class TablikReceiver : TablikConnect
    {
        protected TablikReceiver(Project project, string name, string complect)
            : base(project, name, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
    }
}