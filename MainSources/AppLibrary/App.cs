using BaseLibrary;
using CommonTypes;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : BaseApp
    {
        public App(string code) 
            : base(code, new AppIndicator()) { }
    }
}