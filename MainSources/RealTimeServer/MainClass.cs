using ProvidersLibrary;

namespace RealTimeServer
{
    //Главный класс сервера реального времени
    public class MainClass
    {
        public MainClass()
        {
            ProvidersFactory = new ProvidersFactory();
        }

        //Фабрика провайдеров
        public ProvidersFactory ProvidersFactory { get; private set; }
    }
}