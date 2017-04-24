using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Calculation
{
    //Поток для работы в реальном времени
    public class RealTimeThread : ExternalLogger
    {
        public RealTimeThread(Project project)
            : base(project)
        {
            Project = project;
        }

        //Проект
        public Project Project { get; private set; }

        //Словарь контроллеров источников
        private readonly DicS<SourceController> _sourcesControllers = new DicS<SourceController>();
        public DicS<SourceController> SourcesControllers { get { return _sourcesControllers; } }
        //Словарь контроллеров приемников
        private readonly DicS<ReceiverController> _receiversControllers = new DicS<ReceiverController>();
        public DicS<ReceiverController> ReceiversControllers { get { return _receiversControllers; } }


    }
}