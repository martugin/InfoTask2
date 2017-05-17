using Calculation;
using CommonTypes;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Проект наладчика
    public class CalibratorProject : AppProject
    {
        public CalibratorProject(App app, string projectDir)
            : base(app, projectDir)
        {
            SourceConnect = sourceConnect;
            Sources.Add(sourceConnect.Code, new SchemeConnect(ProviderType.Source, sourceConnect.Code, sourceConnect.Complect));
            ArchiveConnect = archiveConnect;
            Receivers.Add(archiveConnect.Code, new SchemeConnect(ProviderType.Receiver, archiveConnect.Code, archiveConnect.Complect));
        }

        //Поток чтения данных
        public RealTimeThread ReadThread { get; private set; }
        //Поток записи в архив
        public RealTimeThread ArchiveThread { get; private set; }
        //Поток, возвращающий значения
        public RealTimeThread ReturnThread { get; private set; }

        //Соединения с источником и архивом
        public SourceConnect SourceConnect { get; private set; }
        public ReceiverConnect ArchiveConnect { get; private set; }
        //Прокси для возвращения значений
        public ProxyConnect ReturnConnect { get; private set; }

        //Открытие потоков
        public void OpenThreads(double periodSeconds, double lateSeconds)
        {
            var t1 = OpenRealTimeThread(1, "Source", periodSeconds, lateSeconds);
            t1.Sources.Add("Source", new SourceConnect(t1.Logger, "Source", So));
            OpenRealTimeThread(2, "Archive", periodSeconds);
            OpenRealTimeThread(3, "Return", periodSeconds);
        }
    }
}