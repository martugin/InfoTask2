using BaseLibrary;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : DataApp
    {
        public App(string code) 
            : base(code, new AppIndicator()) { }

        //Создать соединение для получения клона
        public ClonerConnect CreateCloner(string complect, string providerCode, string inf)
        {
            var con = new ClonerConnect(this, complect);
            var pr = ProvidersFactory.CreateProvider(this, providerCode, inf);
            con.JoinProvider(pr);
            return con;
        }
    }
}