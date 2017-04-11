using ComLaunchers;

namespace InfoTaskLauncherTest
{
    //“есовый лаунчер, поекты груз€тс€ без указани€ каталогов
    public class TestItLauncher : ItLauncher
    {
        //“естова€ загрузка проекта без указани€ каталога
        internal void LoadProjectByCode(string projectCode)
        {
            Project = new TestAppProject(this, projectCode);
        }        
    }
}