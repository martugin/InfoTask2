using System;

namespace BaseLibrary
{
    //Логгер
    public class Logger : ILogger
    {
        public Logger(LoggerStability stability = LoggerStability.Single)
        {
            Stability = stability;
        }

        //Ссылка на историю
        public IHistory History { get; set; }
        
        //Текущие команды разных типов
        internal Command Command { get; set; }
        internal LogCommand LogCommand { get; set; }
        internal ProgressCommand ProgressCommand { get; set; }
        internal IndicatorTextCommand IndicatorTextCommand { get; set; }
        internal CollectCommand CollectCommand { get; set; }
        internal KeepCommand KeepCommand { get; set; }
        
         //Режим работы потока
        protected internal LoggerStability Stability { get; private set; }

        //События отображения индикатора
        public event EventHandler<EventArgs> ShowTextedIndicator;
        public event EventHandler<EventArgs> ShowTimedIndicator;
        //Событие скрытия индикатора
        public event EventHandler<EventArgs> HideIndicator;
        //Событие изменения текста на табло
        public event EventHandler<ChangeTabloTextEventArgs> ChangeTabloText;
        //Событие обновления периода для индикатора
        public event EventHandler<ChangePeriodEventArgs> ChangePeriod;
        //События установки и снятия режима индикатора с обратным отсчетом времени
        public event EventHandler<SetProcentTimedEventArgs> SetProcentTimed;
        public event EventHandler<EventArgs> SetProcentUsual;
        //Событие изменения уровня индикатора
        public event EventHandler<ChangeProcentEventArgs> ChangeProcent;

        //Аргументы события изменения текста табло
        protected readonly ChangeTabloTextEventArgs TabloArgs = new ChangeTabloTextEventArgs();

        //Событие прерывания выполнения
        public event EventHandler<EventArgs> ExecutionFinished;

        //Вызов события прерывания
        internal void CallExecutionFinished()
        {
            if (ExecutionFinished != null)
                ExecutionFinished(this, new EventArgs());
        }

        //Вызов событий отображения индикаторов
        internal void CallShowTextedIndicator()
        {
            lock (_tabloLocker)
                if (ShowTextedIndicator != null)
                    ShowTextedIndicator(this, new EventArgs());        
            
        }
        internal void CallShowIndicatorTimed()
        {
            lock (_tabloLocker)
                if (ShowTimedIndicator != null)
                    ShowTimedIndicator(this, new EventArgs());
            
        }
        internal void CallHideIndicator()
        {
            lock (_tabloLocker)
                if (HideIndicator != null)
                    HideIndicator(this, new EventArgs());    
        }

        //Включение и отключение индикатора с режимом отсчета обратного времени
        internal void CallSetProcentTimed(DateTime endTime)
        {
            if (SetProcentTimed != null)
                SetProcentTimed(this, new SetProcentTimedEventArgs(endTime));
        }
        internal void CallSetProcentUsual()
        {
            if (SetProcentUsual != null)
                SetProcentUsual(this, new EventArgs());
        }

        //Процент индикатора
        private double _tabloProcent;
        public double TabloProcent
        {
            get { lock (_tabloLocker) return _tabloProcent; }
            internal set
            {
                lock (_tabloLocker)
                {
                    if (_tabloProcent == value) return;
                    _tabloProcent = value;
                    if (ChangeProcent != null)
                        ChangeProcent(this, new ChangeProcentEventArgs(value));
                }
            }
        }
        
        //Три уровня текста на форме индикатора
        //Текст нулевого уровня задается в ProgressCommand
        //Текст первого уровня задается в LogCommand
        //Текст второго уровня задается в CommandProgressText
        internal string TabloText(int number)
        {
            lock (_tabloLocker)
                return TabloArgs.TabloText[number];
        }
        public void SetTabloText(int number, string text)
        {
            lock (_tabloLocker)
            {
                if (TabloArgs.TabloText[number] == text) return;
                TabloArgs.TabloText[number] = text;
                if (ChangeTabloText != null) ChangeTabloText(this, TabloArgs);
            }
        }

        //Задать период обработки
        protected internal void SetPeriod(DateTime begin, DateTime end, string mode = "")
        {
            lock (_tabloLocker)
            {
                _periodBegin = begin;
                _periodEnd = end;
                _periodMode = mode;
                if (ChangePeriod != null)
                    ChangePeriod(this, new ChangePeriodEventArgs(begin, end, mode));
            }
        }

        //Начало и конец текущего периода обработки
        private DateTime _periodBegin = Different.MinDate;
        public DateTime PeriodBegin
        {
            get { lock (_tabloLocker) return _periodBegin; }
        }
        private DateTime _periodEnd = Different.MinDate;
        public DateTime PeriodEnd
        {
            get { lock (_tabloLocker) return _periodEnd; }
        }

        //Режим (Выравнивание, Синхроннный, Разовый и т.п.)
        private string _periodMode;
        public string PeriodMode
        {
            get { lock (_tabloLocker) return _periodMode; }
        }
        
        //Объекты блокировки
        private readonly object _tabloLocker = new object();
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

        //Запуск простой комманды
        public Command Start(double startProcent, double finishProcent)
        {
            return Command = new Command(this, Command, startProcent, finishProcent);
        }
        //Завершение простой команды
        public Command Finish(string results = "")
        {
            return Command == LogCommand 
                ? FinishLog(results) 
                : FinishCommand(Command);
        }

        //Запуск команды логирования
        public LogCommand StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            FinishCommand(LogCommand);
            Command = LogCommand = new LogCommand(this, Command, startProcent, finishProcent, name, context, pars);
            return LogCommand;
        }
        public LogCommand StartLog(string name, string context = "", string pars = "")
        {
            return StartLog(Procent, 100, name, context, pars);
        }
        //Завершение команды логирования
        public LogCommand FinishLog(string results = null)
        {
            if (!results.IsEmpty()) LogCommand.Results = results;
            return (LogCommand)FinishCommand(LogCommand);
        }
        //Присвоить результаты в команду логирования
        public void SetLogCommandResults(string results)
        {
            if (LogCommand != null)
                LogCommand.Results = results;
        }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public ProgressCommand StartProgress(string text, //Текст 0-го уровня для формы индикатора
                                                                  string name, //Имя комманды
                                                                  string pars = "", //Параметры команды
                                                                  DateTime? endTime = null) //Если не null, то время конца обратного отсчета
        {
            FinishCommand(ProgressCommand);
            Command = ProgressCommand = new ProgressCommand(this, Command, text, name, pars, endTime);
            return ProgressCommand;
        }
        //С указанием периода обработки
        public ProgressCommand StartProgress(DateTime begin, DateTime end, //Период обработки
                                                                  string mode, //Режим обработки
                                                                  string name, //Имя комманды
                                                                  string pars = "", //Параметры команды
                                                                  DateTime? endTime = null) //Если не null, то время конца обратного отсчета
        {
            FinishCommand(ProgressCommand);
            Command = ProgressCommand = new ProgressCommand(this, Command, begin, end, mode, name, pars, endTime);
            return ProgressCommand;
        }

        //Завершение команды логирования в SuperHistory
        public ProgressCommand FinishProgress()
        {
            return (ProgressCommand)FinishCommand(ProgressCommand);
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand StartIndicatorText(double startProcent, double finishProcent, string text)
        {
            FinishCommand(IndicatorTextCommand);
            Command = IndicatorTextCommand = new IndicatorTextCommand(this, Command, startProcent, finishProcent, text);
            return IndicatorTextCommand;
        }
        public IndicatorTextCommand StartIndicatorText(string text)
        {
            return StartIndicatorText(Procent, 100, text);
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand FinishIndicatorText()
        {
            return (IndicatorTextCommand)FinishCommand(IndicatorTextCommand);
        }

        //Запуск команды, колекционирущей ошибки
        public CollectCommand StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                              bool isCollect) //Формировать общую ошибку
        {
            FinishCommand(CollectCommand);
            CollectedResults = null;
            Command = CollectCommand = new CollectCommand(this, Command, isWriteHistory, isCollect);
            return CollectCommand;
        }
        //Завершение команды, колекционирущей ошибки
        public CollectCommand FinishCollect(string results = null)
        {
            if (results != null) CollectedResults = results;
            return (CollectCommand)FinishCommand(CollectCommand);
        }
        //Присвоить результат команды Collect
        public void SetCollectCommandResults(string results)
        {
            CollectedResults = results;
        }
        
        //Итоговое сообщение об ошибке
        public string CollectedErrorMessage { get; internal set; }
        //Результат выполнения комманды Collect
        public string CollectedResults { get; internal set; }
        
        //Запуск команды, которая копит ошибки, но не выдает их во вне
        public KeepCommand StartKeep(double startProcent, double finishProcent)
        {
            FinishCommand(KeepCommand);
            Command = KeepCommand = new KeepCommand(this, Command, startProcent, finishProcent);
            return KeepCommand;
        }
        public KeepCommand StartKeep()
        {
            return StartKeep(Procent, 100);
        }
        public KeepCommand FinishKeep()
        {
            return (KeepCommand)FinishCommand(KeepCommand);
        }

        //Ошибка, накопленная KeepCommand
        public string KeepedError { get { return KeepCommand.ErrorMessage; } }

        //Запуск команды, обрамляющей опасную операцию
        public DangerCommand StartDanger(double startProcent, double finishProcent, 
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerStability stability, //Минимальная LoggerStability, начиная с которой выполняется более одного повторения операции
                                        string errMess, //Сообщение об ошибке 
                                        string repeatMess, //Сообщение о повторе
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            Command = new DangerCommand(this, Command, startProcent, finishProcent, repetitions, stability, errMess, repeatMess, useThread, errWaiting);
            return (DangerCommand) Command;
        }
        public DangerCommand StartDanger(int repetitions, LoggerStability stability, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0)
        {
            return StartDanger(Procent, 100, repetitions, stability, errMess, repeatMess, useThread, errWaiting);
        }

        //Завершить указанную команду и всех детей
        protected Command FinishCommand(Command command) 
        {
            CheckBreak();
            if (command != null) command.Finish();
            return command;
        }

        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
            CheckBreak();
            if (History != null) 
                History.WriteEvent(description, pars);
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
    }
}