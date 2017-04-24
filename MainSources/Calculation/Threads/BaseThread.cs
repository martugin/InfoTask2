using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Calculation
{
    //Базовый класс для всех потоков
    public class BaseThread : ExternalLogger
    {
        public BaseThread(ProcessProject processProject)
            : base(processProject.Logger, processProject.Context, processProject.ProgressContext)
        {
            Project = processProject.Project;
        }

        //Проект
        public ServerProject Project { get; private set; }
    }

    //----------------------------------------------------------------------------------------------
    //Поток, содержащий ссылки на соединения и модули
    public class DataThread : BaseThread
    {
        public DataThread(ProcessProject processProject, int id, string name, string description)
            : base(processProject)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        //Номер потока
        public int Id { get; private set; }
        //Имя и описание потока
        public string Name { get; private set; }
        public string Description { get; private set; }

        //Словарь модулей
        private readonly DicS<Module> _modules = new DicS<Module>();
        public IDicSForRead<Module> Modules { get { return _modules; } }

        //Словарь соединений источников
        private readonly DicS<SourceConnect> _sources = new DicS<SourceConnect>();
        public IDicSForRead<SourceConnect> Sources { get { return _sources; } }
        //Словарь соединений приемников
        private readonly DicS<ReceiverConnect> _receivers = new DicS<ReceiverConnect>();
        public IDicSForRead<ReceiverConnect> Receivers { get { return _receivers; } }

        //Добавить модуль
        public Module AddModule(string code)
        {
            return _modules.Add(code, new Module(this, code));
        }
        //Очистить список молулей
        public void ClearModules()
        {
            _modules.Clear();
            _sources.Clear();
            _receivers.Clear();
        }
    }
}