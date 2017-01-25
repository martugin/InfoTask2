using System;

namespace BaseLibrary
{
    //Логгер
    public class Logg
    {
        public Logg(LoggerDangerness dangerness)
        {
            Dangerness = dangerness;
        }

        //Ссылка на историю
        public IHistory History { get; set; }
        
        //Текущие команды разных типов
        internal Comm Command { get; set; }
        internal CommLog CommandLog { get; set; }
        internal CommProgress CommandProgress { get; set; }
        internal CommProgressText CommandProgressText { get; set; }
        internal CommCollect CommandCollect { get; set; }

         //Уровень важности безошибочности по отношению к быстроте
        public LoggerDangerness Dangerness { get; private set; }

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
                }
                //ToDo событие
            }
        }
        //Отображать индикатор на табло
        private bool _showProcent;
        public bool ShowProcent
        {
            get { lock (_tabloLocker) return _showProcent; }
            internal set
            {
                lock (_tabloLocker)
                {
                    if (_showProcent == value) return;
                    _showProcent = value;
                }
                //ToDo событие
            }
        }
        
        //Три уровня текста на форме индикатора
        //Текст нулевого уровня задается в CommandProgress
        //Текст первого уровня задается в CommandLog
        //Текст второго уровня задается в CommandProgressText
        private readonly string[] _tabloText = new [] {"", "", ""};
        public string TabloText(int number)
        {
            lock (_tabloLocker)
                return _tabloText[number];
        }
        public void SetTabloText(int number, string text)
        {
            lock (_tabloLocker)
                _tabloText[number] = text;
            //ToDo событие
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
        public Comm Start(double startProcent, double finishProcent)
        {
            return Command = new Comm(this, Command, startProcent, finishProcent);
        }
        public Comm Start()
        {
            return Start(0, 100);
        }
        //Завершение простоя команды
        public Comm Finish()
        {
            var c = Command;
            FinishCommand(c);
            return c;
        }

        //Запуск команды логирования
        public CommLog StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            FinishCommand(CommandLog);
            Command = CommandLog = new CommLog(this, Command, startProcent, finishProcent, name, context, pars);
            return CommandLog;
        }
        public CommLog StartLog(string name, string context = "", string pars = "")
        {
            return StartLog(0, 100, name, context, pars);
        }
        //Завершение команды логирования
        public CommLog FinishLog(string results = "")
        {
            var c = CommandLog;
            FinishCommand(c, results);
            return c;
        }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public CommProgress StartProgress(string text, string name, string pars = "")
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommProgress(this, Command, text, name, pars);
            return CommandProgress;
        }
        //Завершение команды логирования в SuperHistory
        public CommProgress FinishProgress()
        {
            var c = CommandProgress;
            FinishCommand(c);
            return c;
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public CommProgressText StartProgressText(double startProcent, double finishProcent, string text)
        {
            FinishCommand(CommandProgressText);
            Command = CommandProgressText = new CommProgressText(this, Command, startProcent, finishProcent, text);
            return CommandProgressText;
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public CommProgressText StartProgressText()
        {
            var c = CommandProgressText;
            FinishCommand(c);
            return c;
        }

        //Запуск команды, колекционирущей ошибки
        public CommCollect StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                         bool isCollect) //Формировать общую ошибку
        {
            FinishCommand(CommandCollect);
            Command = CommandCollect = new CommCollect(this, Command, isWriteHistory, isCollect);
            return CommandCollect;
        }
        //Завершение команды, колекционирущей ошибки
        public CommCollect FinishCollect()
        {
            var c = CommandCollect;
            FinishCommand(c);
            return c;
        }

        //Запуск команды, обрамляющей опасную операцию
        public CommDanger StartDanger(double startProcent, double finishProcent, 
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerDangerness dangerness, //Минимальная LoggerDangerness, начиная с которой выполняется более одного повторения операции
                                        string errMess, //Сообщение об ошибке 
                                        string repeatMess, //Сообщение о повторе
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            Command = new CommDanger(this, Command, startProcent, finishProcent, repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
            return (CommDanger) Command;
        }
        public CommDanger StartDanger(int repetitions, LoggerDangerness dangerness, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0)
        {
            return StartDanger(0, 100, repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
        }

        //Завершить указанную команду и всех детей
        protected void FinishCommand(Comm command, string results = "") 
        {
            CheckBreak();
            if (command == null) return;
            command.Finish(results);
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