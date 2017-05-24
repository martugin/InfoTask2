using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Приложение с возможностью управления обработкой
    public class ProcessApp : BaseApp
    {
        public ProcessApp(string code, IIndicator indicator)
            : base(code, indicator)
        {
            ProvidersFactory = new ProvidersFactory();
        }

        //Коструктор для тестов
        protected internal ProcessApp()
        {
            ProvidersFactory = new ProvidersFactory();
        }

        //Фабрика провайдеров
        public ProvidersFactory ProvidersFactory { get; private set; }
    }
}