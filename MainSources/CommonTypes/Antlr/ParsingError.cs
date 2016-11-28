using Antlr4.Runtime;
using BaseLibrary;

namespace CommonTypes
{
    //Ошибка разбора выражения
    public class ParsingError
    {
        public ParsingError(string fieldName, string errMess, IToken token)
        {
            FieldName = fieldName;
            ErrMess = errMess;
            Token = token;
            if (token != null)
            {
                Lexeme = Token.Text;
                Line = Token.Line;
                Pos = Token.Column;
            }
        }

        public ParsingError(string fieldName, string errMess, string lexeme, int line, int pos, IToken token = null)
        {
            FieldName = fieldName;
            ErrMess = errMess;
            Lexeme = lexeme;
            Line = line;
            Pos = pos;
            Token = token;
        }

        //Сообщение
        public string ErrMess { get; private set; }
        //Токен
        internal IToken Token { get; private set; }
        //Текст токена 
        public string Lexeme { get; private set; }
        //Имя разбираемого поля
        public string FieldName { get; private set; }
        //Номер строки
        public int Line { get; private set; }
        //Положение в строке
        public int Pos { get; private set; }

        //Запись в строку
        public override string ToString()
        {
            return ErrMess + (Lexeme.IsEmpty() ? "" : (", '" + Lexeme.Trim() + "'")) + (FieldName.IsEmpty() ? "" : (" (" +FieldName + ", строка: " + Line + ", позиция: " + (Pos + 1) + ")"));
        }
    }
}