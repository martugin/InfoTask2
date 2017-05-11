using AppLibrary;

namespace InfoTaskLauncherTest
{
    //Тестовый проект
    public class TestProject : AppProject
    {
        public TestProject(TestApp app, string code, string name = "")
            : base(app, null)
        {
            Initialize(code, name);
        }
    }
}