using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Модуль, имеющий ссылку на каталог
    public class DataModule : BaseModule
    {
        public DataModule(SchemeProject project, string code) 
            : base(project, code)
        {
            Project = project;
        }

        //Проект
        public SchemeProject Project { get; private set; }
        //Каталог модуля в проекте
        public string Dir { get { return Project.Dir.EndDir() + @"\Modules\" + Code + "\\"; } }
    }
}