using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLibrary
{
    //Кооманда, обрамляющая вызов клиентских операций
    public class CommCollect : Comm
    {
        public CommCollect(Logg logger, Comm parent, double startProcent, double finishProcent) 
            : base(logger, parent, startProcent, finishProcent) { }

        //Список ошибок 
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();

        //Добавить ошибку
        public virtual void AddError(ErrorCommand error)
        {
            bool isFound = false;
            foreach (var err in _errors)
                if (err.EqualsTo(error))
                    isFound = true;
            if (!isFound) _errors.Add(error);
        }

        //Совокупное сообщение об ошибках
        public string ErrorMessage(bool addContext = true, //добавлять контекст ошибки
                                                 bool addParams = true, //добавлять параметры
                                                 bool addErrType = true) //добавлять подписи Ошибка или Предупреждение
        {
            if (_errors == null || _errors.Count == 0) return "";

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