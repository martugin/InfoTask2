using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLibrary
{
    //Кооманда, обрамляющая вызов клиентских операций или запись в ErrorsList
    public class CommCollect : Comm
    {
        public CommCollect(Logg logger, Comm parent) 
            : base(logger, parent, 0, 100) { }

        //Список ошибок 
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();

        //Добавить ошибку
        public override void AddError(ErrorCommand err)
        {
            //ToDo запись в ErrorsList
            bool isFound = false;
            foreach (var e in _errors)
                if (e.EqualsTo(err))
                    isFound = true;
            if (!isFound) _errors.Add(err);
            base.AddError(err);
        }

        //Запуск операции, обрамляемой данной командой
        public override Comm Run(Func<string> func)
        {
            string res = "";
            try
            {
                res = func();
            }
            catch (BreakException)
            {
                while (Logger.Command != this)
                    Logger.Command.FinishCommand(null, true);
                FinishCommand(null, true);
                Logger.WasBreaked = false;
                return this;
            }
            catch (Exception ex)
            {
                AddError(new ErrorCommand("Ошибка", ex));
            }
            return Finish(res);
        }

        //Завершение команды
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            _results = results;
            base.FinishCommand(results, isBreaked);
            Logger.CommandCollect = null;
        }

        //Результаты выполнения операции
        private string _results;

        //Совокупное сообщение об ошибках
        public string ErrorMessage(bool addContext = true, //добавлять контекст ошибки
                                                 bool addParams = true, //добавлять параметры
                                                 bool addErrType = true) //добавлять подписи Ошибка или Предупреждение
        {
            if (_errors == null || _errors.Count == 0) return _results;

            var sb = new StringBuilder();
            if (!_results.IsEmpty()) sb.Append(_results).Append(Environment.NewLine);
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