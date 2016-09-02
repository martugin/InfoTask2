using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using BaseLibrary;

namespace CommonTypes
{
    //Накопление данных в процессе разбора (ошибки и т.п.)
    public class ParsingKeeper 
    {
        //Имя разбираемого поля
        public string FieldName { get; protected set; }
        //Установить разбираемое поле
        public virtual void SetFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        //Список ошибок
        private readonly List<ParsingError> _errors = new List<ParsingError>();
        public List<ParsingError> Errors { get { return _errors; } }

        //Добавить ошибку в список
        public ParsingError AddError(string errMess, IToken token)
        {
            var err = new ParsingError(FieldName, errMess, token);
            Errors.Add(err);
            return err;
        }
        public ParsingError AddError(string errMess, ITerminalNode terminal)
        {
            if (terminal == null)
                return AddError(errMess, (IToken)null);
            return AddError(errMess, terminal.Symbol);
        }
        public ParsingError AddError(string errMess, string lexeme, int line, int pos, IToken token = null)
        {
            var err = new ParsingError(FieldName, errMess, lexeme, line, pos, token);
            Errors.Add(err);
            return err;
        }

        //Накапливаемое сообщение об ошибке
        public string ErrMess
        {
            get
            {
                var s = "";
                foreach (var err in Errors)
                    s += err.ToString();
                return s;
            }
        }

        //Обработка токена целого числа
        public NodeConst GetIntConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return new NodeConst(null, 0);
            int res;
            if (!int.TryParse(terminal.Symbol.Text, out res))
                AddError("Недопустимое целое число", terminal);
            if (res == 1) return new NodeConst(terminal, true);
            if (res == 0) return new NodeConst(terminal, false);
            return new NodeConst(terminal, res);
        }

        //Обработка токена действительного числа 
        public NodeConst GetRealConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return new NodeConst(null, 0.0);
            var token = terminal.Symbol;
            var d = token.Text.ToDouble();
            if (double.IsNaN(d))
            {
                AddError("Недопустимое число с плавающей точкой", token);
                d = 0;
            }
            return new NodeConst(terminal, d);
        }

        //Обработка токена временной константы
        public NodeConst GetTimeConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return new NodeConst(null, Different.MinDate);
            var token = terminal.Symbol;
            var t = token.Text.ToDateTime();
            if (t == Different.MinDate)
                AddError("Недопустимое выражение времени", token);
            return new NodeConst(terminal, t);
        }

        //Обработка токена строковой константы
        public NodeConst GetStringConst(ITerminalNode terminal)
        {
            if (terminal == null || terminal.Symbol == null)
                return new NodeConst(null, "");
            return new NodeConst(terminal, terminal.Symbol.Text);
        }
    }
}