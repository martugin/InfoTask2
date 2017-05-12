using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaseLibrary;
using Calculation;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Базовый класс для всех потоков
    public abstract class BaseThread : ExternalLogger
    {
        protected BaseThread(ProcessProject project, int id, string name)
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
        //Список модулей в порядке обсчета
        private readonly List<CalcModule> _modulesOrder = new List<CalcModule>();
        public List<CalcModule> ModulesOrder { get { return _modulesOrder; } }
        
        //Словарь всех соединений
        private readonly DicS<ProviderConnect> _connects = new DicS<ProviderConnect>();
        public DicS<ProviderConnect> Connects { get { return _connects; } }
        //Словарь соединений источников
        private readonly DicS<SourceConnect> _sources = new DicS<SourceConnect>();
        public DicS<SourceConnect> Sources { get { return _sources; } }
        //Словарь соединений приемников
        private readonly DicS<ReceiverConnect> _receivers = new DicS<ReceiverConnect>();
        public DicS<ReceiverConnect> Receivers { get { return _receivers; } }
        
        //Очистить списки модулей и соединений
        public void ClearModules()
        {
            _modules.Clear();
            _connects.Clear();
            _sources.Clear();
            _receivers.Clear();
        }

        //Добавить модуль в поток
        public CalcModule AddModule(string code)
        {
            var m = new CalcModule(Project, code);
            Modules.Add(code, m);
            var sm = Project.Modules[code];
            foreach (var ccode in sm.LinkedConnectsCodes.Values)
            {
                var con = Connects[ccode];
                if (!Connects.ContainsKey(ccode))
                {
                    var scon = Project.Connects[ccode];
                    con = Project.ProvidersFactory.CreateConnect(Project.App, scon.Type, scon.Code, scon.Complect, Project.Code);
                    con = Connects.Add(code, con);
                    con.JoinProvider(Project.ProvidersFactory.CreateProvider(Project.App, scon.ProviderCode, scon.ProviderInf, Project.Code));
                }
                if (con.Type == ProviderType.Source)
                    m.LinkedSources.Add(Sources.Add(code, (SourceConnect)con));
                if (con.Type == ProviderType.Receiver)
                    m.LinkedReceivers.Add(Receivers.Add(code, (ReceiverConnect)con));
            }

            foreach (var mcode in sm.LinkedModulesCodes.Values)
                if (!Modules.ContainsKey(mcode))
                    m.LinkedModules.Add(AddModule(mcode));
            return m;
        }

        //Пришла команда остановить процесс
        private readonly object _finishLocker = new object();
        private bool _isFinishing;

        //Запуск процесса
        public void StartProcess()
        {
            new Task(Run).Start();
        }

        //Завершение процесса 
        public void FinishProcess()
        {
            lock (_finishLocker)
                _isFinishing = true;
        }

        //Прерывание процесса
        public void BreakProcess()
        {
            
        }

        //Команда, выполняемая в потоке
        private void Run()
        {
            while (true)
            {
                lock (_finishLocker)
                    if (_isFinishing)
                    {
                        _isFinishing = false;
                        break;
                    }
                RunWaiting();
                RunCycle();
            }
        }

        //Выполение одного цикла 
        protected abstract void RunCycle();
        //Ожидание следующего цикла
        protected abstract void RunWaiting();
    }
}