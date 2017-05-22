using System;
using System.Threading;

namespace BaseLibrary
{
    //Логгер
    public class Logger : ILogger, IDisposable
    {
        public Logger(LoggerStability stability = LoggerStability.Single)
        {
            Stability = stability;
        }

        public Logger(IHistory history, IIndicator indicator, LoggerStability stability = LoggerStability.Single)
        {
            History = history;
            Indicator = indicator;
            Stability = stability;
        }

        //Ссылка на историю
        private IHistory _history;

        public IHistory History
        {
            get { return _history; } 
            set 
            { 
                _history = value;
                _history.Logger = this;
            }
        }
        //Ссылка на индикатор
        public IIndicator Indicator { get; protected internal set; }

        //Текущие команды разных типов
        internal CollectCommand CollectCommand { get; set; }
        internal PeriodCommand PeriodCommand { get; set; }
        internal ProgressCommand ProgressCommand { get; set; }
        internal LogCommand LogCommand { get; set; }
        internal IndicatorTextCommand IndicatorTextCommand { get; set; }
        internal KeepCommand KeepCommand { get; set; }
        internal Command Command { get; set; }
        
        //Режим работы потока
        public LoggerStability Stability { get; private set; }

        //Три уровня текста на форме индикатора
        //Текст нулевого уровня задается в ProgressCommand
        //Текст первого уровня задается в LogCommand
        //Текст второго уровня задается в CommandProgressText
        public void SetTabloText(int number, string text)
        {
            if (Indicator != null)
                Indicator.ChangeTabloText(number, text);
        }

        //Объекты блокировки
        private readonly object _breakLocker = new object();
        
        //Пришла команда Break
        private bool _wasBreaked;
        internal bool WasBreaked 
        { 
            get { lock (_breakLocker) return _wasBreaked;}
            set { lock (_breakLocker) _wasBreaked = value; } 
        }

        //Прервать выполнение
        public void Break()
        {
            if (CollectCommand != null)
                WasBreaked = true;
        }

        //Вызвать BreakException
        protected internal void CheckBreak()
        {
            if (WasBreaked)
                throw new BreakException();
        }

        //-----

        //Запуск простой комманды
        public Command Start(double startProcent, double finishProcent)
        {
            return Command = new Command(this, Command, startProcent, finishProcent);
        }
        public Command Start()
        {
            return Command = new Command(this, Command, Procent, Procent);
        }
        //Завершение простой команды
        public Command Finish(string results = "")
        {
            return Command == LogCommand 
                ? FinishLog(results) 
                : FinishCommand(Command);
        }

        //Завершить указанную команду и всех детей
        private Command FinishCommand(Command command)
        {
            CheckBreak();
            if (command != null) command.Finish();
            return command;
        }

        //-----

        //Запуск команды, колекционирущей ошибки
        public CollectCommand StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                              bool isCollect) //Формировать общую ошибку
        {
            FinishCommand(CollectCommand);
            FinishCommand(PeriodCommand);
            FinishCommand(ProgressCommand);
            FinishCommand(LogCommand);
            FinishCommand(IndicatorTextCommand);
            CollectedResults = null;
            Command = CollectCommand = new CollectCommand(this, Command, isWriteHistory, isCollect);
            return CollectCommand;
        }
        //Завершение команды, колекционирущей ошибки
        public CollectCommand FinishCollect(string results = null)
        {
            if (results != null && CollectCommand != null) 
                CollectedResults = CollectCommand.AddResult(results);
            return (CollectCommand)FinishCommand(CollectCommand);
        }
        //Присвоить результат команды Collect
        public void AddCollectResult(string result)
        {
            if (CollectCommand != null)
                CollectedResults = CollectCommand.AddResult(result);
        }

        //Событие прерывания выполнения
        public event EventHandler<EventArgs> ExecutionFinished;
        //Вызов события прерывания
        internal void CallExecutionFinished()
        {
            if (!IsAsynchCommandStarted) return;
            IsAsynchCommandStarted = false;
            if (ExecutionFinished != null)
                ExecutionFinished(this, new EventArgs());
        }

        //Итоговое сообщение об ошибке
        public string CollectedError { get; internal set; }
        //Результат выполнения комманды Collect
        public string CollectedResults { get; internal set; }
        //Команда запущена асинхронно
        internal bool IsAsynchCommandStarted { get; set; }

        //Запускает команду Collect и дожидается ее завершения
        public void RunSyncCommand(Action action)
        {
            StartCollect(false, true).Run(action);
        }
        //Запускает команду Collect. Оповещение о завершении команды через событие Finished
        public void RunAsyncCommand(Action action)
        {
            IsAsynchCommandStarted = true;
            new Thread(() => StartCollect(false, true).Run(action)).Start();
        }
        //То же самое. только с запуском вложенной PeriodCommand
        public void RunSyncCommand(DateTime beg, DateTime en, Action action)
        {
            RunSyncCommand(() => { StartPeriod(beg, en); action(); });
        }
        public void RunAsyncCommand(DateTime beg, DateTime en, Action action)
        {
            RunAsyncCommand(() => { StartPeriod(beg, en); action(); });
        }

        //-----

        //Команда, задающая период обработки
        public PeriodCommand StartPeriod(DateTime begin, DateTime end, string mode = "") //Начало, конец, режим
        {
            FinishCommand(PeriodCommand);
            FinishCommand(ProgressCommand);
            FinishCommand(LogCommand);
            FinishCommand(IndicatorTextCommand);
            Command = PeriodCommand = new PeriodCommand(this, Command, begin, end, mode);
            return PeriodCommand;
        }
        public PeriodCommand FinishPeriod()
        {
            return (PeriodCommand)FinishCommand(PeriodCommand);
        }

        //Начало, конец и режим текущего периода обработки
        public DateTime PeriodBegin
        {
            get { return PeriodCommand == null ? Static.MinDate : PeriodCommand.Begin; }
        }
        public DateTime PeriodEnd
        {
            get { return PeriodCommand == null ? Static.MaxDate : PeriodCommand.End; }
        }
        public string PeriodMode
        {
            get { return PeriodCommand == null ? null : PeriodCommand.Mode; }
        }

        //-----

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public ProgressCommand StartProgress(string name, //Имя команды
                                                                  string pars = "", //Параметры команды
                                                                  string context = null, //Конекст команды
                                                                  DateTime? endTime = null) //Если не null, то время конца обратного отсчета
        {
            FinishCommand(ProgressCommand);
            FinishCommand(LogCommand);
            FinishCommand(IndicatorTextCommand);
            Command = ProgressCommand = new ProgressCommand(this, Command, name, pars, context, endTime);
            return ProgressCommand;
        }
        
        //Завершение команды логирования в SuperHistory
        public ProgressCommand FinishProgress()
        {
            return (ProgressCommand)FinishCommand(ProgressCommand);
        }

        //-----

        //Запуск команды логирования
        public LogCommand StartLog(double startProcent, double finishProcent, string name, string pars = "", string context = null)
        {
            FinishCommand(LogCommand);
            FinishCommand(IndicatorTextCommand);
            Command = LogCommand = new LogCommand(this, Command, startProcent, finishProcent, name, pars, context);
            return LogCommand;
        }
        public LogCommand StartLog(string name, string pars = "", string context = null)
        {
            return StartLog(Procent, Procent, name, pars, context);
        }
        //Завершение команды логирования
        public LogCommand FinishLog(string results = null)
        {
            if (!results.IsEmpty()) LogCommand.Results = results;
            return (LogCommand)FinishCommand(LogCommand);
        }
        //Присвоить результаты в команду логирования
        public void SetLogResults(string results)
        {
            if (LogCommand != null)
                LogCommand.Results = results;
        }

        //-----

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand StartIndicatorText(double startProcent, double finishProcent, string text)
        {
            FinishCommand(IndicatorTextCommand);
            Command = IndicatorTextCommand = new IndicatorTextCommand(this, Command, startProcent, finishProcent, text);
            return IndicatorTextCommand;
        }
        public IndicatorTextCommand StartIndicatorText(string text)
        {
            return StartIndicatorText(Procent, Procent, text);
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand FinishIndicatorText()
        {
            return (IndicatorTextCommand)FinishCommand(IndicatorTextCommand);
        }

        //-----

        //Запуск команды, которая копит ошибки, но не выдает их во вне
        public KeepCommand StartKeep(double startProcent, double finishProcent)
        {
            FinishCommand(KeepCommand);
            Command = KeepCommand = new KeepCommand(this, Command, startProcent, finishProcent);
            return KeepCommand;
        }
        public KeepCommand StartKeep()
        {
            return StartKeep(Procent, Procent);
        }
        public KeepCommand FinishKeep()
        {
            return (KeepCommand)FinishCommand(KeepCommand);
        }

        //Ошибка, накопленная KeepCommand
        public string KeepedError { get { return KeepCommand.ErrorMessage; } }

        //-----

        //Запуск команды, обрамляющей опасную операцию
        public DangerCommand StartDanger(double startProcent, double finishProcent, 
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerStability stability, //Минимальная LoggerStability, начиная с которой выполняется более одного повторения операции
                                        string eventMess, //Сообщение о событии для записи в историю
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            AddEvent(eventMess);
            Command = new DangerCommand(this, Command, startProcent, finishProcent, repetitions, stability, eventMess, useThread, errWaiting);
            return (DangerCommand) Command;
        }
        public DangerCommand StartDanger(int repetitions, LoggerStability stability, string eventMess, bool useThread = false, int errWaiting = 0)
        {
            return StartDanger(Procent, Procent, repetitions, stability, eventMess, useThread, errWaiting);
        }

        //-----
        
        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
            if (Stability != LoggerStability.RealTimeFast)
            {
                CheckBreak();
                if (History != null)
                    History.WriteEvent(description, pars);    
            }
        }
        //Добавляет событие в лог c указанием процентов текущей комманды
        public void AddEvent(string description, string pars, double procent)
        {
            AddEvent(description, pars);
            Procent = procent;
        }
        public void AddEvent(string description, double procent)
        {
            AddEvent(description, "", procent);
        }

        //Добавляет ошибку в комманду, лог и отображение, err - ошибка, 
        private void AddError(CommandError err)
        {
            CheckBreak();
            if (Command != null)
                Command.AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddError(string text, Exception ex = null, string pars = "", string context = null)
        {
            string cx = context ?? (LogCommand == null ? "" : LogCommand.Context);
            var err = new CommandError(text, ex, pars, cx);
            AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddWarning(string text, Exception ex = null, string pars = "", string context = null)
        {
            string cx = context ?? (LogCommand == null ? "" : LogCommand.Context);
            var err = new CommandError(text, ex, pars, cx, CommandQuality.Warning);
            AddError(err);
        }

        //Процент текущей комманды
        public double Procent
        {
            get { return Command == null ? 0 : Command.Procent; }
            set
            {
                CheckBreak();
                if (Command != null) 
                    Command.Procent = value;
            }
        }

        //Очистка ресурсов
        public void Dispose()
        {
            try { DisposeLogger(); } catch {} 
            try
            {
                FinishCollect();
                FinishPeriod();
                FinishProgress();
                FinishLog();    
            }
            catch { }
            try
            {
                if (History != null)
                    History.Close();
            }
            catch { }
        }

        //Метод для реализации очистки ресурсов от наследника
        protected virtual void DisposeLogger() {}
    }
}