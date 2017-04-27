using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Приложение с доступом к проектам и исходным данным
    public class DataApp : BaseApp
    {
        public DataApp(string code, IIndicator indicator)
            : base(code, indicator)
        {
            ProvidersFactory = new ProvidersFactory();
        }

        //Фабрика провайдеров
        public ProvidersFactory ProvidersFactory { get; private set; }
    }
}