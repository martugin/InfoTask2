using CommonTypes;

namespace ClientLibrary
{
    //Приложение - клиент
    public class ClientApp : BaseApp
    {
        public ClientApp(string code)
            : base(code, null) { }

        //Коструктор для тестов
        protected internal ClientApp() { }
    }
}