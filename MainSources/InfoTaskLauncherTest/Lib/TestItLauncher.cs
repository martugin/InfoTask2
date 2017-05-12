using ComLaunchers;

namespace InfoTaskLauncherTest
{
    //������� �������, ������ �������� ��� �������� ���������
    public class TestItLauncher : ItLauncher
    {
        //������������� ��� ������ ����������
        public void TestInitialize(string appCode) //��� ����������
        {
            Initialize(appCode);
            App = new TestApp(appCode);
        }

        //�������� �������� ������� ��� �������� ��������
        internal LauncherProject LoadProjectByCode(string projectCode)
        {
            return new LauncherProject(new TestProject((TestApp) App, projectCode));
        }        
    }
}