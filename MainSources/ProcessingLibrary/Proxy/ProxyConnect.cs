using BaseLibrary;
using CommonTypes;

namespace ProcessingLibrary
{
    //Прокси-соединение
    public class ProxyConnect : IWritingConnect, IReadingConnect
    {
        public ProxyConnect(string code)
        {
            Code = code;
        }

        //Код
        public string Code { get; private set; }

        //Словари связанных входящих и исходящих соединений
        private readonly DicS<IReadingConnect> _inConnects = new DicS<IReadingConnect>();
        public DicS<IReadingConnect> InConnects { get { return _inConnects; } }
        private readonly DicS<IReadingConnect> _outConnects = new DicS<IReadingConnect>();
        public DicS<IReadingConnect> OutConnects { get { return _outConnects; } }
        
        //Словарь сигналов
        protected readonly DicS<ProxySignal> Signals = new DicS<ProxySignal>();
        //Сигналы для внешного пользования
        public IDicSForRead<IReadSignal> ReadingSignals { get { return Signals; } }
        public IDicSForRead<IWriteSignal> WritingSignals { get { return Signals; } }

        //Объект для блокировки значений
        protected readonly object ValuesLocker = new object();

        //Очистить список сигналов
        public void ClearSignals()
        {
            lock (ValuesLocker)
                Signals.Clear();
        }

        //Добавить сигнал
        public virtual ProxySignal AddSignal(IReadingConnect connect, IReadSignal signal) //Оборачиваемый сигнал
        {
            lock (ValuesLocker)
                return Signals.Add(connect.Code + "." + signal.Code, new ProxySignal(connect.Code, signal));
        }
        
        //Получить значения в прокси
        public virtual void WriteValues()
        {
            lock (ValuesLocker)
                foreach (var signal in Signals.Values)
                    signal.WriteValue();
        }

        //Получить значения из прокси
        public virtual void ReadValues()
        {
            lock (ValuesLocker)
                foreach (var signal in Signals.Values)
                    signal.ReadValue();
        }
    }
}