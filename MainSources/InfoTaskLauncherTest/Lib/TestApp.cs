using AppLibrary;
using BaseLibraryTest;

namespace InfoTaskLauncherTest
{
    //Тестовое приложение
    public class TestApp : App
    {
        public TestApp(string code) 
            : base(code)
        {
            Indicator = new TestIndicator();
            History = new TestHistory();
        }
    }
}