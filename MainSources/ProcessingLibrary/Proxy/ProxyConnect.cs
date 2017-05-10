using BaseLibrary;
using CommonTypes;

namespace ProcessingLibrary
{
    //Прокси-соединение
    public class ProxyConnect : IWritingConnect, IReadingConnect
    {
        public ProxyConnect(IConnect connect) //Оборачиваемое соединение
        {
            _connect = connect;
        }

        //Соединение
        private readonly IConnect _connect;
        //Код
        public string Code { get { return _connect.Code; } }

        //Словарь сигналов
        private readonly DicS<ProxySignal> _signals = new DicS<ProxySignal>();
        //Сигналы для внешного пользования
        public IDicSForRead<IReadSignal> ReadingSignals { get { return _signals; } }
        public IDicSForRead<IWriteSignal> WritingSignals { get { return _signals; } }

        //Добавить сигнал
        public ProxySignal AddSignal(ISignal signal) //Оборачиваемый сигнал
        {
            return _signals.Add(signal.Code, new ProxySignal(signal));
        }
    }
}