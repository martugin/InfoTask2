using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Управление обработкой мгновенных данных (расчеты и т.п.)
    public class ProcessProject : ExternalLogger
    {
        public ProcessProject(ServerProject project) 
            : base(project.App, project.Code, project.Code)
        {
            Project = project;
        }

        //Проект
        public ServerProject Project { get; private set; }
    }
}