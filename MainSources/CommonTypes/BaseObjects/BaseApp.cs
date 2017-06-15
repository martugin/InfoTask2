using System.IO;
using BaseLibrary;

namespace CommonTypes
{
    //Одно приложение InfoTask
    public abstract class BaseApp : Logger
    {
        protected BaseApp(string code, IIndicator indicator)
            : base(indicator)
        {
            Code = code;
        }

        //Инициализация
        public void Init()
        {
            History = ItStatic.CreateHistory(this, Code + '\\' + Code);
            LocalDir = ItStatic.InfoTaskDir() + @"LocalData\" + Code + @"\";
            var dir = new DirectoryInfo(LocalDir);
            if (!dir.Exists) dir.Create();
        }

        //Тестовая инициализация
        public void InitTest()
        {
            History = new TestHistory(this);
            LocalDir = ItStatic.InfoTaskDir() + @"TestsRun\LocalData\" + Code + @"\";
            var dir = new DirectoryInfo(LocalDir);
            if (!dir.Exists) dir.Create();
        }

        //Код приложения
        public string Code { get; private set; }
        //Каталог проекта в LocalData
        public string LocalDir { get; protected set; }

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