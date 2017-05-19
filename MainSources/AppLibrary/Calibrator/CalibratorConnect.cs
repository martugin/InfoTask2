using CommonTypes;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Одно соединение с источником для калибратора (вместе с прокси)
    public class CalibratorConnect
    {
        public CalibratorConnect(RealTimeThread thread, string connectCode, string complect, string providerCode, string providerInf)
        {
            SourceConnect = new SourceConnect(thread.Logger, connectCode, complect);
            SourceConnect.JoinProvider(thread.ProvidersFactory.CreateProvider(thread.Logger, providerCode, providerInf));
            UserProxyConnect = new ProxyConnect("UserProxy");
            ArchiveProxyConnect = new QueuedProxyConnect("ArchiveProxy");
        }

        //Соединение с источником
        public SourceConnect SourceConnect { get; private set; }
        //Прокси для выдачи пользователю и в архив
        public ProxyConnect UserProxyConnect { get; private set; }
        public ProxyConnect ArchiveProxyConnect { get; private set; }

        //Очистить список сигналов
        public void ClearSignals()
        {
            ArchiveProxyConnect.ClearSignals();
            UserProxyConnect.ClearSignals();
            SourceConnect.ClearSignals();
        }

        //Добавить сигнал, возвращает сигнал прокси для пользователя
        public ProxySignal AddSignal(string fullCode, DataType dataType, SignalType signalType, string infObject, string infOut, string infProp)
        {
            var sig = SourceConnect.AddSignal(fullCode, dataType, signalType, infObject, infOut, infProp);
            ArchiveProxyConnect.AddSignal(SourceConnect, new QueuedProxySignal(SourceConnect.Code, sig));
            return UserProxyConnect.AddSignal(SourceConnect, new ProxySignal(SourceConnect.Code, sig));
        }
    }
}