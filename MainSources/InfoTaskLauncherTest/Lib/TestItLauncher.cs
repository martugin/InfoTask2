using ComLaunchers;

namespace InfoTaskLauncherTest
{
    //������� �������, ������ �������� ��� �������� ���������
    public class TestItLauncher : ItLauncher
    {
        //�������� �������� ������� ��� �������� ��������
        internal void LoadProjectByCode(string projectCode)
        {
            Project = new TestAppProject(this, projectCode);
        }        
    }
}