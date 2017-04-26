using BaseLibrary;
using Calculation;
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
        public IDicSForRead<CalcModule> Modules { get { return _modules; } }

        //Словарь соединений источников
        private readonly DicS<SourceConnect> _sources = new DicS<SourceConnect>();
        public IDicSForRead<SourceConnect> Sources { get { return _sources; } }
        //Словарь соединений приемников
        private readonly DicS<ReceiverConnect> _receivers = new DicS<ReceiverConnect>();
        public IDicSForRead<ReceiverConnect> Receivers { get { return _receivers; } }

        //Добавить модуль
        public CalcModule AddModule(string code)
        {
            return _modules.Add(code, new CalcModule(Project, code));
        }
        //Добавить соединение
        public ProviderConnect AddConnect()
        {
            
        }

        //Очистить список молулей
        public virtual void ClearConnects()
        {
            _modules.Clear();
            _sources.Clear();
            _receivers.Clear();
        }
    }
}