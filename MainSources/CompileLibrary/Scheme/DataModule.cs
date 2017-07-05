using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Модуль, имеющий ссылку на каталог
    public abstract class DataModule : BaseModule
    {
        protected DataModule(SchemeProject project, string code) 
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