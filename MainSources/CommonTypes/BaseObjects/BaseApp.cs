using BaseLibrary;

namespace CommonTypes
{
    //Одно приложение InfoTask
    public class BaseApp : Logger
    {
        public BaseApp(string code, IIndicator indicator)
            : base(ItStatic.CreateHistory(code + '\\' + code), indicator)
        {
            Code = code;
        }

        //Коструктор для тестов
        protected internal BaseApp() : base(new TestHistory(), new TestIndicator())
        { 
            Code = "Test";
        }

        //Код приложения
        public string Code { get; private set; }

        //Todo реализовать через VerSyn
        //Номер програмного продукта
        public int ProductNumber
        {
            get { return 1; }
        }
        //Проверка активации приложения
        public bool IsActivated
        {
            get { return true; }
        }
    }
}