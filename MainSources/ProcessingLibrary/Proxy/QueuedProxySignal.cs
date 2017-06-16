using System.Collections.Generic;
using CommonTypes;

namespace ProcessingLibrary
{
    //Сигнал для прокси, накапливающий очередь значений
    public class QueuedProxySignal : ProxySignal
    {
        public QueuedProxySignal(string connectCode, IReadSignal signal)
            : base(connectCode, signal) { }
        
        //Очередь значений
        private readonly Queue<IReadMean> _values = new Queue<IReadMean>();

        //Взять значение из исходного сигнала в прокси
        public override void WriteValue()
        {
            _values.Enqueue((IReadMean)InSignal.OutValue.Clone());
        }

        //Прочитать значение из прокси в OutValue
        public override IReadMean ReadValue()
        {
            return OutValue = _values.Dequeue();
        }
    }
}