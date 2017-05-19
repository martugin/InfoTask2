using CommonTypes;
using ProcessingLibrary;

namespace AppLibrary
{
    //Проект наладчика
    public class CalibratorProject : AppProject
    {
        public CalibratorProject(App app, string projectDir)
            : base(app, projectDir) { }

        //Поток чтения данных
        public RealTimeThread ReadThread { get; private set; }
        //Поток записи в архив
        public ProxyThread ArchiveThread { get; private set; }
        //Поток, возвращающий значения
        public RealTimeThread UserThread { get; private set; }

        //Прокси для возвращения значений
        public ProxyConnect ReturnConnect { get; private set; }

        //Открытие потоков
        public void OpenThreads(double periodSeconds, double lateSeconds)
        {
            ReadThread = OpenRealTimeThread(1, "Source", periodSeconds, lateSeconds);
            var aproxy = new QueuedProxyConnect("ArchiveProxy");
            ReturnConnect = new ProxyConnect("UserProxy");
            ReadThread.Proxies.Add(aproxy.Code, aproxy);
            ReadThread.Proxies.Add(ReturnConnect.Code, ReturnConnect);
            foreach (var s in SchemeSources.Values)
            {
                var con = (IReadingConnect)ReadThread.AddConnect(s.Code);
                aproxy.InConnects.Add(s.Code, con);
                ReturnConnect.InConnects.Add(s.Code, con);
            }
            ArchiveThread = OpenProxyThread(2, "Archive", aproxy);
            ArchiveThread.AddConnect("Archive");
            UserThread = OpenRealTimeThread(3, "Return", periodSeconds);
            UserThread.Proxies.Add(ReturnConnect.Code, ReturnConnect);
        }
    }
}