using ProcessingLibrary;

namespace ServiceLibrary
{
    //Базовый класс для служб
    public class ServiceApp : ProcessApp
    {
        public ServiceApp(string code)
            : base(code, null) { }

        //Коструктор для тестов
        protected internal ServiceApp() { }
    }
}