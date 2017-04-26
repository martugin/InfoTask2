using BaseLibrary;
using CommonTypes;

namespace ProcessingLibrary
{
    //Поток для работы в реальном времени
    public class RealTimeThread : BaseThread
    {
        public RealTimeThread(BaseProject project)
            : base(project)
        {
            Project = project;
        }

        //Проект
        public BaseProject Project { get; private set; }

        //Словарь контроллеров источников
        private readonly DicS<SourceController> _sourcesControllers = new DicS<SourceController>();
        public DicS<SourceController> SourcesControllers { get { return _sourcesControllers; } }
        //Словарь контроллеров приемников
        private readonly DicS<ReceiverController> _receiversControllers = new DicS<ReceiverController>();
        public DicS<ReceiverController> ReceiversControllers { get { return _receiversControllers; } }


    }
}