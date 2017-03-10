using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    public abstract class ReceiverOut : IContextable 
    {
        protected ReceiverOut(BaseReceiver receiver)
        {
            Receiver = receiver;
        }

        //Ссылка на приемник
        protected BaseReceiver Receiver { get; private set; }
        //Код объекта - контекст для формирования ошибок
        public string Context { get; internal set; }

        //Основной сигнал объекта
        public ReceiverSignal ValueSignal { get; set; }
        //Список сигналов объекта
        private readonly HashSet<ReceiverSignal> _signals = new HashSet<ReceiverSignal>();
        protected HashSet<ReceiverSignal> Signals { get { return _signals; } }

        //Добавить к объекту сигнал, если такого еще не было
        internal ReceiverSignal AddSignal(ReceiverSignal sig)
        {
            var s = AddNewSignal(sig);
            if (!Signals.Contains(s))
                Signals.Add(s);
            return s;
        }
        protected virtual ReceiverSignal AddNewSignal(ReceiverSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}