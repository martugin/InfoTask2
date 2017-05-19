using CommonTypes;

namespace ProcessingLibrary
{
    public class QueuedProxyConnect : ProxyConnect
    {
        public QueuedProxyConnect(string code) 
            : base(code) { }

        //Добавить сигнал
        public override ProxySignal AddSignal(IReadingConnect connect, IReadSignal signal) //Оборачиваемый сигнал
        {
            lock (ValuesLocker)
                return Signals.Add(connect.Code + "." + signal.Code, new QueuedProxySignal(connect.Code, signal));
        }

        //Количество значений в очереди 
        private int _valuesCount;
        public int ValuesCount
        {
            get { lock (ValuesLocker) return _valuesCount; }
        }

        //Получить значения в прокси
        public override void WriteValues()
        {
            lock (ValuesLocker)
            {
                _valuesCount++;
                foreach (var signal in Signals.Values)
                    signal.WriteValue();
            }
        }

        //Получить значения из прокси
        public override void ReadValues()
        {
            lock (ValuesLocker)
            {
                _valuesCount--;
                foreach (var signal in Signals.Values)
                    signal.ReadValue();
            }
        }
    }
}