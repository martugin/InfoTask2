using BaseLibrary;
using ProvidersLibrary;

namespace Calculation
{
    //Поток расчетов
    public class CalcThread : DataThread
    {
        //Если поток у проекта один
        public CalcThread(ProcessProject processProject) 
            : base(processProject) { }

        //Если потоков у проекта много
        public CalcThread(ProcessProject processProject, int id, string name, string description) : base(processProject)
        {
            Id = id;
            Name = name;
            Description = description;
            ThreadLogger = new Logger(Project.App.CreateHistory(Project.Code + Id), new ServiceIndicator(), LoggerStability.Periodic); 
        }

        //Логгер для запуска выполнения в отдельном потоке
        public Logger ThreadLogger { get; private set; }
    }
}