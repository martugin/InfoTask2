using ComLaunchers;

namespace InfoTaskLauncherTest
{
    //������� �������, ������ �������� ��� �������� ���������
    public class TestItLauncher : ItLauncher
    {
        //�������� �������� ������� ��� �������� ��������
        internal void LoadProjectByCode(string projectCode)
        {
            _project = new TestAppProject(this, projectCode);
        }        
    }
}