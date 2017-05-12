using CommonTypes;

namespace ClientLibrary
{
    //Приложение - клиент
    public class ClientApp : BaseApp
    {
        public ClientApp(string code)
            : base(code, null)
        {
            History = CreateHistory(code);
        }
    }
}