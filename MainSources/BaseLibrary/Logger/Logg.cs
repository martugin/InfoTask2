using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    //Логгер
    public class Logg
    {

        //Ссылка на историю
        internal IHistory History { get; set; }
        
        //Текущие команды разных типов
        internal Comm Command { get; set; }
        internal CommLog CommandLog { get; set; }
        internal CommLog CommandSubLog { get; set; }
        internal CommProgress CommandProgress { get; set; }
        internal CommCollect CommandCollect { get; set; }
        //Стек опаных команд
        private readonly Stack<CommDanger> _commandsDanger = new Stack<CommDanger>();
        public Stack<CommDanger> CommandsDanger { get { return _commandsDanger; } }
    
        //Проценты для старта комманд, если они явно не зананы
        private double StartProcent { get { return Command == null ? 0 : Command.Procent; } }
        private double FinishProcent { get { return Command == null ? 100 : Command.Procent; } }

        //Запуск простой комманды
        public Comm Start(double startProcent, double finishProcent)
        {
            return Command = new Comm(this, Command, startProcent, finishProcent);
        }
        public Comm Start()
        {
            return Start(StartProcent, FinishProcent);
        }
        public Comm Start(Action action, double startProcent, double finishProcent)
        {
            var c = Start(startProcent, finishProcent);
            action();
            return c.Finish();
        }
        public Comm Start(Action action)
        {
            return Start(action, StartProcent, FinishProcent);
        }
        
        //Запуск команды логирования
        public CommLog StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            FinishCommand(CommandLog);
            Command = CommandLog = new CommLog(this, Command, startProcent, finishProcent, name, context, pars);
            if (History != null) History.WriteStart(CommandLog);
            return CommandLog;
        }
        public CommLog StartLog(string name, string context = "", string pars = "")
        {
            FinishCommand(CommandLog);
            return StartLog(StartProcent, FinishProcent, name, context, pars);
        }
        public CommLog StartLog(Action action, double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            var c = StartLog(startProcent, finishProcent, name, context, pars);
            try { action(); }
            catch (BreakException) { throw;}
            catch (Exception ex)
            {
                AddError("Ошибка при выполнении комманды " + name, ex, pars, context);
            }
            return (CommLog)c.Finish();
        }
        public CommLog StartLog(Action action, string name, string context = "", string pars = "")
        {
            FinishCommand(CommandLog);
            return StartLog(action, StartProcent, FinishProcent, name, context, pars);
        }

        //Запуск команды логирования в SubHistory
        public CommLog StartSubLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            FinishCommand(CommandSubLog);
            Command = CommandSubLog = new CommLog(this, Command, startProcent, finishProcent, name, context, pars);
            if (History != null) History.WriteStartSub(CommandSubLog);
            return CommandSubLog;
        }
        public CommLog StartSubLog(string name, string context = "", string pars = "")
        {
            FinishCommand(CommandSubLog);
            return StartSubLog(StartProcent, FinishProcent, name, pars);
        }
        public CommLog StartSubLog(Action action, double startProcent, double finishProcent, string name, string pars = "")
        {
            var c = StartSubLog(startProcent, finishProcent, name, pars);
            try { action(); }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                AddError("Ошибка при выполнении комманды " + name, ex, pars);
            }
            return (CommLog)c.Finish();
        }
        public CommLog StartSubLog(Action action, string name, string pars = "")
        {
            FinishCommand(CommandSubLog);
            return StartSubLog(action, StartProcent, FinishProcent, name, pars);
        }

        //Запуск команды, отображающей индикатор
        public CommProgress StartProgress(string text)
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommProgress(this, Command, text);
            return CommandProgress;
        }
        public CommProgress StartProgress(Action action, string text)
        {
            var c = StartProgress(text);
            action();
            return (CommProgress)c.Finish();
        }

        //Запуск внешней команды от клиента, колекционирущей ошибки
        public CommCollect StartCollect(Action action, double startProcent, double finishProcent)
        {
            FinishCommand(CommandCollect);
            Command = CommandCollect = new CommCollect(this, Command, startProcent, finishProcent);
            try { action(); }
            catch (Exception ex)
            {
                CommandCollect.AddError(new ErrorCommand("Ошибка", ex));
            }
            return CommandCollect;
        }
        public CommCollect StartCollect(Action action)
        {
            FinishCommand(CommandCollect);
            return StartCollect(action, StartProcent, FinishProcent);
        }

        //Завершить указанную команду и всех детей
        internal void FinishCommand(Comm command, 
                                                  bool isBreaked = false) //Комманда была прервана
        {
            if (command == null) return;
            Comm c = Command;
            while (c != command)
            {
                c.Finish(isBreaked);
                c = c.Parent;
            }
            c.Finish(isBreaked);
        }

        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
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
            if (History != null)
                History.WriteError(err);
            if (Command != null)
                Command.AddQuality(err.Quality);
            //ToDo Danger и LogErrors
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddError(string text, Exception ex = null, string pars = "", string context = null)
        {
            string cx = context ?? (CommandLog == null ? "" : CommandLog.Context);
            var err = new ErrorCommand(text, ex, pars, cx);
            AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddWarning(string text, Exception ex = null, string pars = "", string context = "")
        {
            string cx = context ?? (CommandLog == null ? "" : CommandLog.Context);
            var err = new ErrorCommand(text, ex, pars, cx, CommandQuality.Warning);
            AddError(err);
        }

        //Процент текущей комманды
        public double Procent
        {
            get { return Command == null ? 0 : Command.Procent; }
            set { if (Command != null) Command.Procent = value; }
        }
    }
}