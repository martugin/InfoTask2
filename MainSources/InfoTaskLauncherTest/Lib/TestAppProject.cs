using BaseLibraryTest;
using ComLaunchers;

namespace InfoTaskLauncherTest
{
    internal class TestAppProject : AppProject
    {
        internal TestAppProject(ItLauncher launcher, string projectCode) 
            : base(launcher)
        {
            ProjectCode = projectCode;
            Logger = TestLib.CreateTestLogger();
        }
    }
}