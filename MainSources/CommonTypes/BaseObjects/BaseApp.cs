using BaseLibrary;

namespace CommonTypes
{
    //Одно приложение InfoTask
    public class BaseApp : Logger
    {
        public BaseApp(string code, IIndicator indicator) 
        {
            Code = code;
            History = CreateHistory(code);
            Indicator = indicator;
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

        //Инициализация истории
        public AccessHistory CreateHistory(string historyFilePrefix) //Путь к файлу истории относительно каталога истории прриложения
        {
            return new AccessHistory(
                    ItStatic.InfoTaskDir() + @"LacalData\History\" + Code + "\\" + historyFilePrefix + "History.accdb",
                    ItStatic.TemplatesDir + @"LocalData\History.accdb");
        }
    }
}