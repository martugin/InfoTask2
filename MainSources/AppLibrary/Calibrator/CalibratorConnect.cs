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
            ProxyConnect = new ProxyConnect(SourceConnect);
        }

        //Соединение с источником и прокси
        public SourceConnect SourceConnect { get; private set; }
        public ProxyConnect ProxyConnect { get; private set; }

        //Добавить сигнал
        public ProxySignal AddSignal()
        {
            
        }
    }
}