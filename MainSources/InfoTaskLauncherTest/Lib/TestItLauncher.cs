using ComLaunchers;

namespace InfoTaskLauncherTest
{
    //“есовый лаунчер, поекты груз€тс€ без указани€ каталогов
    public class TestItLauncher : ItLauncher
    {
        //»нициализаци€ дл€ нового приложени€
        public void TestInitialize(string appCode) // од приложени€
        {
            Initialize(appCode);
            App = new TestApp(appCode);
        }

        //“естова€ загрузка проекта без указани€ каталога
        internal LauncherProject LoadProjectByCode(string projectCode)
        {
            return new LauncherProject(new TestProject((TestApp) App, projectCode));
        }        
    }
}