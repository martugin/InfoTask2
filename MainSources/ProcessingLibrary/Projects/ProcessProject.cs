using BaseLibrary;
using Calculation;

namespace ProcessingLibrary
{
    //Управление обработкой мгновенных данных (расчеты и т.п.)
    public class ProcessProject : ExternalLogger
    {
        public ProcessProject(DataProject project) 
            : base(project.App, project.Code, project.Code)
        {
            Project = project;
        }

        //Проект
        public DataProject Project { get; private set; }
    }
}