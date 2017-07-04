using BaseLibrary;
using ProcessingLibrary;
using ProvidersLibrary;
using Tablik;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : ProcessApp
    {
        public App(string code, IIndicator indicator, bool useTablik)
            : base(code, indicator)
        {
            if (useTablik) Tablik = new TablikApp();
        }

        //Создание соединения-клонера и присоединение провайдера
        public ClonerConnect LoadCloner(string providerCode, string providerInf)
        {
            _cloner = new ClonerConnect(this);
            _cloner.JoinProvider(ProvidersFactory.CreateProvider(this, providerCode, providerInf));
            return _cloner;
        }
        private ClonerConnect _cloner;

        //Приложение Tablik
        public TablikApp Tablik { get; private set; } 

        //Очистка ресурсов
        protected override void DisposeLogger()
        {
                   
        }
    }
}