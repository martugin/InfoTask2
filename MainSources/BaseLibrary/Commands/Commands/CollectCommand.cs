using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLibrary
{
    //Команда, обрамляющая вызов клиентских операций или запись в ErrorsList
    public class CollectCommand : Command
    {
        public CollectCommand(Logger logger, Command parent, bool isWriteHistory, bool isCollect)
            : base(logger, parent, 0, 100)
        {
            _isWriteHistory = isWriteHistory;
            _isCollect = isCollect;
        }

        //Записывать ошибки в ErrorsList
        private readonly bool _isWriteHistory;
        //Формировать общую ошибку
        private readonly bool _isCollect;

        //Список ошибок 
        private readonly List<CommandError> _errors = new List<CommandError>();

        //Добавить ошибку
        public override void AddError(CommandError err)
        {
            if (_isWriteHistory && Logger.History != null)
                Logger.History.WriteErrorToList(err);
            if (_isCollect)
            {
                bool isFound = false;
                foreach (var e in _errors)
                    if (e.EqualsTo(err))
                        isFound = true;
                if (!isFound) _errors.Add(err);
            }
            base.AddError(err);
        }

        public override Command Run(Action action)
        {
            return Run(action, null);
        }

        //Запуск операции, обрамляемой данной командой
        public Command Run(Action action,
                                      Action finishAction) //Действие, выполняемое при завершении команды
        {
            try
            {
                action();
            }
            catch (BreakException)
            {
                if (finishAction != null) finishAction();
                while (Logger.Command != this)
                    Logger.Command.FinishCommand(true);
                FinishCommand(true);
                Logger.CallExecutionFinished();
                Logger.WasBreaked = false;
                return this;
            }
            catch (Exception ex)
            {
                AddError(new CommandError("Ошибка", ex));
            }
            if (finishAction != null) finishAction();
            return Finish();
        }

        //Завершение команды
        protected internal override void FinishCommand(bool isBreaked)
        {
            Logger.CollectedErrorMessage = ErrorMessage();
            base.FinishCommand(isBreaked);
            Logger.CollectCommand = null;
        }

        //Совокупное сообщение об ошибках
        public string ErrorMessage(bool addContext = true, //добавлять контекст ошибки
                                                 bool addParams = true, //добавлять параметры
                                                 bool addErrType = true) //добавлять подписи Ошибка или Предупреждение
        {
            if (!_isCollect || _errors == null || _errors.Count == 0) return "";

            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var e in _errors.Where(e => e.Quality == CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType) sb.Append("Ошибка: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty())
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty())
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            foreach (var e in _errors.Where(e => e.Quality != CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType) sb.Append("Предупреждение: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty())
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty())
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            return sb.ToString();
        }
    }
}