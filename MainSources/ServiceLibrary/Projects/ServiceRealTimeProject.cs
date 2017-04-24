using Calculation;
using CommonTypes;

namespace ServiceLibrary
{
    //Проект реального времени для службы
    public class ServiceRealTimeProject : ProcessProject
    {
        public ServiceRealTimeProject(ServerProject project) 
            : base(project) { }
    }
}