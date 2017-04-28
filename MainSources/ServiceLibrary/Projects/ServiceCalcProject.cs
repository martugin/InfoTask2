using Calculation;
using CommonTypes;

namespace ServiceLibrary
{
    //Расчетный проект для службы
    public class ServiceCalcProject : ProcessProject 
    {
        public ServiceCalcProject(ServerProject project) 
            : base(project) { }
    }
}