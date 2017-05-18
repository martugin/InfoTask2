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
            foreach (var s in SchemeSources.Values)
                
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
            
            OpenRealTimeThread(2, "Archive", periodSeconds);
            OpenRealTimeThread(3, "Return", periodSeconds);
        }
    }
}