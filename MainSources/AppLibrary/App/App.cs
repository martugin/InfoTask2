using BaseLibrary;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : ProcessApp
    {
        public App(string code) 
            : base(code, new AppIndicator()) { }

        //Создание соединения-клонера и присоединение провайдера
        public ClonerConnect LoadCloner(string providerCode, string providerInf)
        {
            var con = new ClonerConnect(this);
            con.JoinProvider(ProvidersFactory.CreateProvider(this, providerCode, providerInf));
            return con;
        }
    }
}