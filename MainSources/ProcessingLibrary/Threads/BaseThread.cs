using BaseLibrary;
using Calculation;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Базовый класс для всех потоков
    public class BaseThread : ExternalLogger
    {
        public BaseThread(ProcessProject project, int id, string name)
            : base(project.Logger, project.Context, project.ProgressContext)
        {
            Project = project;
            Id = id;
            Name = name;
            ThreadLogger = new Logger(Project.App.CreateHistory(Project.Code + Id), new ServiceIndicator(), LoggerStability.Periodic); 
        }

        //Логгер для запуска выполнения в отдельном потоке
        public Logger ThreadLogger { get; private set; }

        //Проект
        public ProcessProject Project { get; private set; }

        //Номер потока
        public int Id { get; private set; }
        //Имя потока
        public string Name { get; private set; }

        //Словарь модулей
        private readonly DicS<CalcModule> _modules = new DicS<CalcModule>();
        public DicS<CalcModule> Modules { get { return _modules; } }

        //Словарь соединений источников
        private readonly DicS<SourceConnect> _sources = new DicS<SourceConnect>();
        public DicS<SourceConnect> Sources { get { return _sources; } }
        //Словарь соединений приемников
        private readonly DicS<ReceiverConnect> _receivers = new DicS<ReceiverConnect>();
        public DicS<ReceiverConnect> Receivers { get { return _receivers; } }
        

        //Очистить список молулей
        public virtual void ClearConnects()
        {
            _modules.Clear();
            _sources.Clear();
            _receivers.Clear();
        }
    }
}