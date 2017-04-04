using ProvidersLibrary;

namespace RealTimeServer
{
    //Поток получения архивных данных из истоника в режиме реального времени
    public class RealTimeSourceThread : RealTimeThread
    {
        public RealTimeSourceThread(MainClass main,
                                                     string appCode, //Код приложения
                                                     string project, //Код проекта
                                                     string connCode, //Код соединения
                                                     string complect) //Комплект соединения
            : base(main, appCode, project)
        {
            Connect = (SourceConnect)MainClass.ProvidersFactory.CreateConnect(ProviderType.Source, connCode, complect, this);
        }

        //Соединение с источником
        public SourceConnect Connect { get; private set; }

        //Сдвиг в секундах времени начала и конца периода считывания данных от текущего времени
        public int ReadingShiftBegin { get; private set; }
        public int ReadingShiftEnd { get; private set; }
    }
}