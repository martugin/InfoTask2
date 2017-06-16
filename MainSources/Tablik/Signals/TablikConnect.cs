using System;
using CommonTypes;

namespace Tablik
{
    //Проайдер для компилятора
    public abstract class TablikConnect : BaseConnect
    {
        protected TablikConnect(TablikProject tablik, string code, string complect) 
            : base(tablik.Project.App, code, complect, tablik.Project.Code) { }

        //Загрузка сигналов
        public virtual void LoadSignals()
        {
            StartLog("Загрузка сигналов", null, Type + " " + Code).Run(LoadProvidersSignals);
        }
        protected abstract void LoadProvidersSignals();
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    public class TablikSource : TablikConnect
    {
        protected TablikSource(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}

        //Загрузка сисгналов
        protected override void LoadProvidersSignals()
        {
            
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    public class TablikReceiver : TablikConnect
    {
        protected TablikReceiver(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect) { }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }

        //Загрузка сисгналов
        protected override void LoadProvidersSignals()
        {
            
            
        }
    }
}