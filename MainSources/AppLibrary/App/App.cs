using BaseLibrary;
using CompileLibrary;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : ProcessApp
    {
        public App(string code, IIndicator indicator, bool useTablik)
            : base(code, indicator)
        {
            if (useTablik) FunsChecker = new FunsChecker(FunsCheckType.Calc);
        }

        //Создание соединения-клонера и присоединение провайдера
        public ClonerConnect LoadCloner(string providerCode, string providerInf)
        {
            _cloner = new ClonerConnect(this);
            _cloner.JoinProvider(ProvidersFactory.CreateProvider(this, providerCode, providerInf));
            return _cloner;
        }
        private ClonerConnect _cloner;

        //Список функций для компиляция
        public FunsChecker FunsChecker { get; private set; }
        

        //Очистка ресурсов
        protected override void DisposeLogger()
        {
                   
        }
    }
}