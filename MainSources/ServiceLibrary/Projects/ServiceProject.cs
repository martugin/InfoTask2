using ProcessingLibrary;

namespace ServiceLibrary
{
    //Проект для службы
    public class ServiceProject : ProcessProject
    {
        public ServiceProject(ProcessApp app, string projectDir) 
            : base(app, projectDir) { }
    }
}