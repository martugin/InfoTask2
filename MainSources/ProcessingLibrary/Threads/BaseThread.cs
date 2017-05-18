using System;
using System.Collections.Generic;
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
        Stopped, //Остановлен
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
            State = ThreadState.Stopped;
            Project = project;
            Id = id;
            Name = name;
        }

        //Проект
        public ProcessProject Project { get; private set; }
        //Ссылка на фабрику провайдеров
        public ProvidersFactory ProvidersFactory { get { return ((ProcessApp)Project.App).ProvidersFactory; }}
        
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
            var sm = Project.SchemeModules[code];
            foreach (var ccode in sm.LinkedConnects.Values)
            {
                var con = Connects[ccode];
                if (!Connects.ContainsKey(ccode))
                {
                    var scon = Project.SchemeConnects[ccode];
                    con = ProvidersFactory.CreateConnect(Logger, scon.Type, scon.Code, scon.Complect, Project.Code);
                    con = Connects.Add(code, con);
                    con.JoinProvider(ProvidersFactory.CreateProvider(Logger, scon.ProviderCode, scon.ProviderInf, Project.Code));
                }
                if (con.Type == ProviderType.Source)
                    m.LinkedSources.Add(Sources.Add(code, (SourceConnect)con));
                if (con.Type == ProviderType.Receiver)
                    m.LinkedReceivers.Add(Receivers.Add(code, (ReceiverConnect)con));
            }

            foreach (var mcode in sm.LinkedModules.Values)
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
        
        //Период, который обрабатывается или готовится обрабатываться
        public DateTime ThreadPeriodBegin { get; set; }
        public DateTime ThreadPeriodEnd { get; set; }
        //Время запуска следующего цикла
        protected DateTime NextPeriodStart { get; set; }
        //Время остановки цикла расчета
        public DateTime ThreadFinishTime { get; private set; }

        //Запуск процесса
        public void StartProcess(DateTime startTime, //Начало первого периода обработки
                                             DateTime? finishTime = null) //Окончание последнего периода, если не задано, то бесконечно
        {
            ThreadPeriodBegin = startTime;
            ThreadFinishTime = finishTime ?? Static.MaxDate;
            bool b = false;
            lock (StateLocker)
            {
                if (State == ThreadState.Finishing)
                    State = ThreadState.Run;
                if (State == ThreadState.Stopped)
                {
                    State = ThreadState.Run;
                    b = true;
                }
            }
            if (b) new Task(RunProcess).Start();
        }

        //Завершение процесса 
        public void FinishProcess()
        {
            lock (StateLocker)
                if (State == ThreadState.Run)
                    State = ThreadState.Finishing;
        }

        //Прерывание процесса
        public void BreakProcess()
        {
            lock (StateLocker)
                if (State != ThreadState.Stopped)
                {
                    State = ThreadState.Breaking;
                    Logger.Break();    
                }
        }

        //Проверка на наличие команды Finishing
        protected bool CheckFinishing()
        {
            lock (StateLocker)
                if (State == ThreadState.Finishing)
                {
                    State = ThreadState.Stopped;
                    return true;
                }
            return false;
        }

        //Вызвать событие по завершению процесса
        protected void MakeStopped()
        {
            lock (StateLocker)
                State = ThreadState.Stopped;
            //Todo StopEvent для клиента
        }

        //Весь процесс обработки
        protected virtual void RunProcess()
        {
            try
            {
                Prepare();
                while (!CheckFinishing())
                {
                    Waiting();
                    Cycle();
                    if (!NextPeriod()) break;
                }
                ClearMemory();
                MakeStopped();
            }
            catch (OutOfMemoryException)
            {
                MakeStopped();
                //Todo Restart
                throw;
            }
            catch (BreakException)
            {
                MakeStopped();
                throw;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при работе потока", ex);
            }
        }
        
        //Подготовка потока
        #region Prepare
        protected abstract void Prepare();

        //Очистка памяти после завершения всех циклов обработки
        protected virtual void ClearMemory()
        {
            
        }

        //Загрузка модулей
        protected virtual void LoadModules()
        {
            foreach (var module in ModulesOrder)
                using (StartLog(Procent, Procent + 100.0 / ModulesOrder.Count, "Загрузка модуля", "", module.Code))
                    module.Load();
        }
        #endregion

        //Ожидание
        protected abstract void Waiting();
        //Определение следующего периода обработки, возвращает false, если следующй обработки не будет
        protected virtual bool NextPeriod() { return false; }

        //Цикл обработки
        #region Cycle
        protected abstract void Cycle();

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
                using (StartLog(Procent, Procent + 100.0 / Receivers.Count, "Запись в приемник", "", receiver.Code))
                    receiver.WriteValues();
        }

        //Запись в прокси
        protected virtual void WriteProxies()
        {
            foreach (var proxy in Proxies.Values)
                using (StartLog(Procent, Procent + 100.0 / Proxies.Count, "Запись в прокси", "", proxy.Code))
                    proxy.WriteValues();
        }

        //Очистка значений
        protected virtual void ClearValues()
        {
            using (StartLog(0, 100, "Очистка значений источников"))
                foreach (var source in Sources.Values)
                    source.ClearSignalsValues(false);
        }
        #endregion
    }
}