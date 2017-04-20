namespace CommonTypes
{
    //Одно приложение InfoTask
    public class BaseApp
    {
        public BaseApp(string code)
        {
            Code = code;
        }

        //Код приложения
        public string Code { get; private set; }

        //Todo реализовать через VerSyn
        //Номер програмного продукта
        public static int ProductNumber
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