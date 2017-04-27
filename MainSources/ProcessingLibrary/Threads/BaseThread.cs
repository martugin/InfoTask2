using BaseLibrary;
using Calculation;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Базовый класс для всех потоков
    public class BaseThread : ExternalLogger
    {
        public BaseThread(ProcessProject processProject, int id, string name, string description)
            : base(processProject.Logger, processProject.Context, processProject.ProgressContext)
        {
            Project = processProject.Project;
            Id = id;
            Name = name;
            Description = description;
        }

        //Проект
        public DataProject Project { get; private set; }

        //Номер потока
        public int Id { get; private set; }
        //Имя и описание потока
        public string Name { get; private set; }
        public string Description { get; private set; }

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