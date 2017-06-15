using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Приложение с возможностью управления обработкой
    public abstract class ProcessApp : BaseApp
    {
        protected ProcessApp(string code, IIndicator indicator)
            : base(code, indicator)
        {
            ProvidersFactory = new ProvidersFactory();
        }
        
        //Фабрика провайдеров
        public ProvidersFactory ProvidersFactory { get; private set; }
    }
}