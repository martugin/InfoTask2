using System;

namespace BaseLibrary
{
    //Логгер
    public class Logger
    {
        public Logger(LoggerDangerness dangerness = LoggerDangerness.Single)
        {
            Dangerness = dangerness;
        }

        //Ссылка на историю
        public IHistory History { get; set; }
        
        //Текущие команды разных типов
        internal Command Command { get; set; }
        internal CommandLog CommandLog { get; set; }
        internal CommandProgress CommandProgress { get; set; }
        internal CommandProgressText CommandProgressText { get; set; }
        internal CommandCollect CommandCollect { get; set; }
        internal CommandKeep CommandKeep { get; set; }
        //Накопленная ошибка
        public string KeepedError { get { return CommandKeep.ErrorMessage; } }

         //Уровень важности безошибочности по отношению к быстроте
        public LoggerDangerness Dangerness { get; private set; }

        //Событие отображения индикатора
        public event EventHandler<ShowIndicatorEventArgs> ShowIndicator;
        //Событие скрытия индикатора
        public event EventHandler<EventArgs> HideIndicator;
        //Событие изменения уровня индикатора
        public event EventHandler<ChangeProcentEventArgs> ChangeProcent;
        //Событие изменения текста на табло
        public event EventHandler<ChangeTabloTextEventArgs> ChangeTabloText;

        //Аргументы события изменения текста табло
        protected readonly ChangeTabloTextEventArgs TabloArgs = new ChangeTabloTextEventArgs();
        
        //Отображать индикатор на табло
        private bool _indicatorVisible;
        public bool IndicatorVisible
        {
            get { lock (_tabloLocker) return _indicatorVisible; }
            internal set
            {
                lock (_tabloLocker)
                {
                    if (_indicatorVisible == value) return;
                    _indicatorVisible = value;
                    if (ShowIndicator != null) ShowIndicator(this, new ShowIndicatorEventArgs(value));
                }
            }
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
                    if (ChangeProcent != null) ChangeProcent(this, new ChangeProcentEventArgs(value));
                }
            }
        }
        
        //Три уровня текста на форме индикатора
        //Текст нулевого уровня задается в CommandProgress
        //Текст первого уровня задается в CommandLog
        //Текст второго уровня задается в CommandProgressText
        public string TabloText(int number)
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
            WasBreaked = true;
        }

        //Вызвать BreakException
        internal protected void CheckBreak()
        {
            if (WasBreaked)
                throw new BreakException();
        }

        //Запуск простой комманды
        public Command Start(double startProcent, double finishProcent)
        {
            return Command = new Command(this, Command, startProcent, finishProcent);
        }
        public Command Start()
        {
            return Start(0, 100);
        }
        //Завершение простой команды
        public Command Finish(string results = "")
        {
            if (Command == CommandLog)
                return FinishLog(results);
            return FinishCommand(Command);
        }

        //Запуск команды логирования
        public CommandLog StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            FinishCommand(CommandLog);
            Command = CommandLog = new CommandLog(this, Command, startProcent, finishProcent, name, context, pars);
            return CommandLog;
        }
        public CommandLog StartLog(string name, string context = "", string pars = "")
        {
            return StartLog(0, 100, name, context, pars);
        }
        //Завершение команды логирования
        public CommandLog FinishLog(string results = "")
        {
            return (CommandLog)FinishCommand(CommandLog, results);
        }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public CommandProgress StartProgress(string text, string name, string pars = "")
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommandProgress(this, Command, text, name, pars);
            return CommandProgress;
        }
        //Завершение команды логирования в SuperHistory
        public CommandProgress FinishProgress()
        {
            return (CommandProgress)FinishCommand(CommandProgress);
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public CommandProgressText StartProgressText(double startProcent, double finishProcent, string text)
        {
            FinishCommand(CommandProgressText);
            Command = CommandProgressText = new CommandProgressText(this, Command, startProcent, finishProcent, text);
            return CommandProgressText;
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public CommandProgressText StartProgressText()
        {
            return (CommandProgressText)FinishCommand(CommandProgressText);
        }

        //Запуск команды, колекционирущей ошибки
        public CommandCollect StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                              bool isCollect) //Формировать общую ошибку
        {
            FinishCommand(CommandCollect);
            Command = CommandCollect = new CommandCollect(this, Command, isWriteHistory, isCollect);
            return CommandCollect;
        }
        //Завершение команды, колекционирущей ошибки
        public CommandCollect FinishCollect()
        {
            return (CommandCollect)FinishCommand(CommandCollect);
        }

        //Запуск команды, которая копит ошибки, но не выдает из во вне
        public CommandKeep StartKeep(double startProcent, double finishProcent)
        {
            FinishCommand(CommandKeep);
            Command = CommandKeep = new CommandKeep(this, Command, startProcent, finishProcent);
            return CommandKeep;
        }
        public CommandKeep StartKeep()
        {
            return StartKeep(0, 100);
        }
        public CommandKeep FinishKeep()
        {
            return (CommandKeep)FinishCommand(CommandKeep);
        }
        
        //Запуск команды, обрамляющей опасную операцию
        public CommandDanger StartDanger(double startProcent, double finishProcent, 
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerDangerness dangerness, //Минимальная LoggerDangerness, начиная с которой выполняется более одного повторения операции
                                        string errMess, //Сообщение об ошибке 
                                        string repeatMess, //Сообщение о повторе
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            Command = new CommandDanger(this, Command, startProcent, finishProcent, repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
            return (CommandDanger) Command;
        }
        public CommandDanger StartDanger(int repetitions, LoggerDangerness dangerness, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0)
        {
            return StartDanger(0, 100, repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
        }

        //Завершить указанную команду и всех детей
        protected Command FinishCommand(Command command, string results = "") 
        {
            CheckBreak();
            if (command != null) command.Finish(results);
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
        private void AddError(ErrorCommand err)
        {
            CheckBreak();
            if (Command != null)
                Command.AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddError(string text, Exception ex = null, string pars = "", string context = null)
        {
            string cx = context ?? (CommandLog == null ? "" : CommandLog.Context);
            var err = new ErrorCommand(text, ex, pars, cx);
            AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddWarning(string text, Exception ex = null, string pars = "", string context = null)
        {
            string cx = context ?? (CommandLog == null ? "" : CommandLog.Context);
            var err = new ErrorCommand(text, ex, pars, cx, CommandQuality.Warning);
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