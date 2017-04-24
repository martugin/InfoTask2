using CommonTypes;

namespace ServiceLibrary
{
    //Базовый класс для служб
    public class ServiceApp : BaseApp
    {
        public ServiceApp(string code) 
            : base(code, null) { }
    }
}