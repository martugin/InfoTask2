﻿using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    //Логгер
    public class Logg
    {
        public Logg(IHistory history)
        {
            History = history;
        }

        //Ссылка на историю
        internal IHistory History { get; set; }
        
        //Текущие команды разных типов
        internal Comm Command { get; set; }
        internal CommLog CommandLog { get; set; }
        internal CommSuperLog CommandSuperLog { get; set; }
        internal CommProgress CommandProgress { get; set; }
        internal CommCollect CommandCollect { get; set; }
        //Стек опаных команд
        private readonly Stack<CommDanger> _commandsDanger = new Stack<CommDanger>();
        public Stack<CommDanger> CommandsDanger { get { return _commandsDanger; } }

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
        
        //3 уровня текста на форме индикатора
        private readonly string[] _tabloText = new string[3];
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
            private set { lock (_breakLocker) _wasBreaked = value; } 
        }

        //Прервать выполнение
        public void Break()
        {
            WasBreaked = true;
        }

        //Вызвать BreakException
        internal protected void MakeBreak()
        {
            if (WasBreaked)
            {
                WasBreaked = false;
                throw new BreakException();
            }
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
        
        //Запуск команды логирования в SuperHistory
        public CommSuperLog StartSuperLog(double startProcent, double finishProcent, DateTime begin, DateTime end, string mode, string name, string pars = "")
        {
            FinishCommand(CommandSuperLog);
            Command = CommandSuperLog = new CommSuperLog(this, Command, startProcent, finishProcent, begin, end, mode, name, pars);
            return CommandSuperLog;
        }
        public CommSuperLog StartSuperLog(DateTime begin, DateTime end, string mode, string name, string pars = "")
        {
            return StartSuperLog(0, 100, begin, end, mode, name, pars);
        }
        public CommSuperLog StartSuperLog(double startProcent, double finishProcent, string name, string pars = "")
        {
            return StartSuperLog(startProcent, finishProcent, Different.MinDate, Different.MinDate, "", name, pars);
        }
        public CommSuperLog StartSuperLog( string name, string pars = "")
        {
            return StartSuperLog(0, 100, name, pars);
        }

        //Запуск команды, отображающей индикатор
        public CommProgress StartProgress(string text)
        {
            FinishCommand(CommandProgress);
            Command = CommandProgress = new CommProgress(this, Command, text);
            return CommandProgress;
        }

        //Запуск команды, колекционирущей ошибки
        public CommCollect StartCollect()
        {
            FinishCommand(CommandCollect);
            Command = CommandCollect = new CommCollect(this, Command);
            return CommandCollect;
        }

        //Завершить указанную команду и всех детей
        protected void FinishCommand(Comm command) 
        {
            MakeBreak();
            if (command == null) return;
            command.Finish();
        }

        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
            MakeBreak();
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
            MakeBreak();
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
                MakeBreak();
                if (Command != null) 
                    Command.Procent = value;
            }
        }
    }
}