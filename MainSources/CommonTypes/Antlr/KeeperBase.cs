using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Базовый класс для накопителей и прослушивателей ошибок
    public abstract class KeeperBase
    {
        protected KeeperBase(string fieldName)
        {
            FieldName = fieldName;
        }

        //Имя разбираемого поля
        protected string FieldName { get; private set; }

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
    }
}