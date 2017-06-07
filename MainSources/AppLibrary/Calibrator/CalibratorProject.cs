using CommonTypes;
using ProcessingLibrary;

namespace AppLibrary
{
    //Проект наладчика
    public class CalibratorProject : ProcessProject
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
            ArchiveThread.Proxies.Add("ArchiveProxy", aproxy);
            UserThread = OpenRealTimeThread(3, "Return", periodSeconds);
            UserThread.Proxies.Add(ReturnConnect.Code, ReturnConnect);
        }

        //Открыть поток реального времени
        public RealTimeThread OpenRealTimeThread(int id, string name, double periodSeconds, double lateSeconds = 0)
        {
            var t = new RealTimeThread(this, id, name, null, periodSeconds, lateSeconds);
            Threads.Add(id, t, true);
            return t;
        }

        //Открыть поток реального времени, управляемы очередью прокси
        public ProxyThread OpenProxyThread(int id, string name, QueuedProxyConnect proxy)
        {
            var t = new ProxyThread(this, id, name, null, proxy);
            Threads.Add(id, t, true);
            return t;
        }

        //Запуск потоков
        public void StartThreads()
        {
            ReadThread.StartProcess();
            UserThread.StartProcess();
            ArchiveThread.StartProcess();
        }

        //Останов потоков
        public void StopThreads()
        {
            ReadThread.StopProcess();
            UserThread.StartProcess();
            ArchiveThread.StartProcess();
        }
    }
}