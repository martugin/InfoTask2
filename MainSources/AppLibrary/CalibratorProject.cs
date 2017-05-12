using Calculation;
using CommonTypes;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Проект наладчика
    public class CalibratorProject : AppProject
    {
        public CalibratorProject(App app, string projectDir, SourceConnect sourceConnect, ReceiverConnect archiveConnect)
            : base(app, projectDir)
        {
            SourceConnect = sourceConnect;
            Sources.Add(new SchemeConnect(ProviderType.Source, sourceConnect.Code, sourceConnect.Complect));
            ArchiveConnect = archiveConnect;
            Sources.Add(new SchemeConnect(ProviderType.Receiver, archiveConnect.Code, archiveConnect.Complect));
            Threads.Add(1, ReadThread = new RealTimeThread(this, 1, "Source"));
            Threads.Add(2, ArchiveThread = new RealTimeThread(this, 2, "Archive"));
            Threads.Add(3, ReturnThread = new RealTimeThread(this, 1, "Return"));
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
    }
}