using BaseLibrary;
using CommonTypes;

namespace Integrated
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : BaseApp
    {
        public App(string code) : base(code)
        {
            Indicator = new AppIndicator();
        }

        //Индикатор
        internal AppIndicator Indicator { get; private set; }
    }
}