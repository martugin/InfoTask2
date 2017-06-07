using System.IO;
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
            InitLocalData();
        }

        //Коструктор для тестов
        protected internal BaseApp(string code = "Test") 
            : base(new TestHistory(), new TestIndicator())
        {
            IsTest = true;
            Code = code;
            InitLocalData();
        }

        //Является тестовым
        internal bool IsTest { get; private set; }
        //Код приложения
        public string Code { get; private set; }
        //Каталог проекта в LocalData
        public string LocalDir { get; protected set; }
        //Инициализация каталога в LocalData
        private void InitLocalData()
        {
            LocalDir = ItStatic.InfoTaskDir() + (IsTest ? @"TestsRun" : "") + @"\LocalData\" + Code + @"\";
            var dir = new DirectoryInfo(LocalDir);
            if (!dir.Exists) dir.Create();
        }

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