using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Calculation
{
    //Поток расчетов
    public class CalcThread : ExternalLogger
    {
        //Если поток у проекта один
        public CalcThread(Project project)
        {
            Logger = Project = project;
        }

        //Если потоков у проекта много
        public CalcThread(Project project, int id, string name, string description)
        {
            Project = project;
            Id = id;
            Name = name;
            Description = description;
            Logger = new Logger(project.CreateHistory(@"Threads\History" + Id + ".accdb"), null); //Todo добавить индикатор службы
        }

        //Проект
        public Project Project { get; private set; }
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
            return _modules.Add(code, new CalcModule(this, code));
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