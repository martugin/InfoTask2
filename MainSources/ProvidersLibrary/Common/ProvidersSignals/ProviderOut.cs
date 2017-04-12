using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для выходов всех провайдеров
    public abstract class ProviderOut : IContextable
    {
        protected ProviderOut(Provider provider)
        {
            Provider = provider;
        }

        //Ссылка на провайдер
        protected Provider Provider { get; private set; }
        //Свойства объекта + свойства выхода - контекст для формирования ошибок
        public string Context { get; internal set; }

        //Список сигналов объекта
        private readonly HashSet<ProviderSignal> _signals = new HashSet<ProviderSignal>();
        protected HashSet<ProviderSignal> Signals { get { return _signals; } }

        //Добавить к объекту сигнал, если такого еще не было
        internal void AddSignal(ProviderSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
                Signals.Add(s);
        }

        protected abstract ProviderSignal AddNewSignal(ProviderSignal sig);
    }
}