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
    //Сотояния потока
    public enum ThreadState
    {
        Sopped, //Остановлен
        Run, //Выполняется обработка
        Finishing, //Завершение выполнения (кнопка Стоп)
        Breaking //Прерывание выполнения
    }

    //----------------------------------------------------------------------------------------------------
    //Базовый класс для всех потоков
    public abstract class BaseThread : ExternalLogger
    {
        protected BaseThread(ProcessProject project, int id, string name, IIndicator indicator, LoggerStability stability)
            : base(new Logger(project.App.CreateHistory(project.Code + id), indicator, stability), project.Context, project.ProgressContext)
        {
            State = ThreadState.Sopped;
            Project = project;
            Id = id;
            Name = name;
        }

        //Проект
        public ProcessProject Project { get; private set; }
        //Ссылка на фабрику провайдеров
        public ProvidersFactory ProvidersFactory { get { return ((ProcessApp) Project.App).ProvidersFactory; }}
        
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
        //Словарь всех соединений прокси
        private readonly DicS<ProxyConnect> _proxies = new DicS<ProxyConnect>();
        public DicS<ProxyConnect> Proxies { get { return _proxies; } }

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
                    con = ProvidersFactory.CreateConnect(Logger, scon.Type, scon.Code, scon.Complect, Project.Code);
                    con = Connects.Add(code, con);
                    con.JoinProvider(ProvidersFactory.CreateProvider(Logger, scon.ProviderCode, scon.ProviderInf, Project.Code));
                }
                if (con.Type == ProviderType.Source)
                    m.LinkedSources.Add(Sources.Add(code, (SourceConnect)con));
                if (con.Type == ProviderType.Receiver)
                    m.LinkedReceivers.Add(Receivers.Add(code, (ReceiverConnect)con));
            }

            foreach (var mcode in sm.LinkedModulesCodes.Values)
            {
                if (!Modules.ContainsKey(mcode))
                    AddModule(mcode);
                m.LinkedModules.Add(Modules[mcode]);
            }
            return m;
        }

        //Состояние потока
        protected ThreadState State { get; set; }
        protected readonly object StateLocker = new object();

        //Запуск процесса
        public void StartProcess()
        {
            lock (StateLocker) 
                State = ThreadState.Run;
            new Task(Run).Start();
        }

        //Завершение процесса 
        public void FinishProcess()
        {
            lock (StateLocker)
                State = ThreadState.Finishing;
        }

        //Прерывание процесса
        public void BreakProcess()
        {
            lock (StateLocker)
                State = ThreadState.Breaking;
            Logger.Break();
        }

        //Команда, выполняемая в потоке
        protected abstract void Run();

        //Обрамление для запуска основных команд потока
        private void RunThreadCommand(Action action)
        {
            try
            {
                action();
            }
            catch (OutOfMemoryException)
            {
                lock (StateLocker)
                    State = ThreadState.Sopped;
                throw;
            }
            catch (BreakException)
            {
                lock (StateLocker)
                    State = ThreadState.Sopped;
                throw;
            }
        }
        //Запуск основных команд потока
        protected void RunPrepare()
        {
            using ( StartProgress("Подготовка потока"))
                RunThreadCommand(Prepare);
        }
        protected void RunCycle()
        {
            RunThreadCommand(Cycle);
        }
        protected void RunWaiting()
        {
            RunThreadCommand(Waiting);
        }

        //Выполение одного цикла 
        protected void Cycle()
        {
            using (Start(0, 40)) 
                ReadSources();
            using (Start(40, 70))
                ClaculateModules();
            using (Start(70, 90))
                WriteReceivers();
            using (Start(90, 100))
                ClearValues();
        }

        //Чтение из источников
        protected virtual void ReadSources()
        {
            foreach (var source in Sources.Values)
                using (StartLog(Procent, Procent + 100.0 / Sources.Count, "Чтение из источника", "", source.Code))
                    source.GetValues();
        }

        //Вычисления
        protected virtual void ClaculateModules()
        {
            foreach (var module in ModulesOrder)
                using (StartLog(Procent, Procent + 100.0 / Sources.Count, "Вычисление значения модуля", "", module.Code))
                    module.Calculate();
        }

        //Запись в приемники
        protected virtual void WriteReceivers()
        {
            foreach (var receiver in Receivers.Values)
                using (StartLog(Procent, Procent + 70.0 / Receivers.Count, "Запись в приемник", "", receiver.Code))
                    receiver.WriteValues();
            foreach (var proxy in Proxies.Values)
                using (StartLog(Procent, Procent + 30.0 / Proxies.Count, "Запись в прокси", "", proxy.Code))
                    proxy.GetValues();
        }

        //Очистка значений
        protected virtual void ClearValues()
        {
            using (StartLog(0, 100, "Очистка значений источников"))
                foreach (var source in Sources.Values)
                    source.ClearSignalsValues(false);
        }

        //Ожидание следующего цикла
        protected virtual void Waiting()
        {

        }

        //Подготовка потока
        protected virtual void Prepare()
        {

        }
    }
}