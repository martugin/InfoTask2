﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary
{
    //Команда, которая копит ошибки, но не выдает из во вне (только в лог)
    public class KeepCommand : Command
    {
        internal KeepCommand(Logger logger, Command parent, double startProcent, double finishProcent) 
            : base(logger, parent, startProcent, finishProcent)
        {
        }

        //Список ошибок
        private readonly List<CommandError> _errors = new List<CommandError>();

        //Добавить ошибку 
        public override void AddError(CommandError err)
        {
            if (Logger.History != null)
                Logger.History.WriteError(err);
            AddQuality(err.Quality);
            _errors.Add(err);
        }

        //Совокупное сообщение об ошибках
        public string ErrorMessage
        {
            get
            {
                string s = "";
                foreach (var err in _errors)
                {
                    if (s != "") s += Environment.NewLine;
                    s += err.ToString();
                }
                return s;
            }
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            base.FinishCommand(isBreaked);
            Logger.KeepCommand = null;
        }
    }
}