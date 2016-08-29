using Antlr4.Runtime;

namespace CommonTypes
{
    //Прослушиватель ошибок лексера
    public class LexerErrorListener : KeeperBase, IAntlrErrorListener<int>
    {
        public LexerErrorListener(string fieldName, string fieldValue)
            : base(fieldName)
        {
            FieldValue = fieldValue;
        }

        //Разбираемая строка
        protected string FieldValue { get; private set; }

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var lexer = (Lexer)recognizer;
            const string mess = "Недопустимая последовательность символов";
            if (lexer.Token != null)
            {
                var t = lexer.Token;
                AddError(mess, t.Text + FieldValue[lexer.CharIndex], t.Line, t.Column, t);
            }
            else AddError(mess, lexer.Text + FieldValue[lexer.CharIndex], line, charPositionInLine);
        }
    }

    //-------------------------------------------------------------------------------------------------
    //Прослушиватель ошибок парсера
    public class ParserErrorListener : KeeperBase, IAntlrErrorListener<IToken>
    {
        public ParserErrorListener(string fieldName)
            : base(fieldName) { }

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            //var p = (Parser) recognizer;
            string mess = "Ошибка разбора выражения";
            IToken token = offendingSymbol;
            if (offendingSymbol != null)
            {
                if (offendingSymbol.Text == "<EOF>")
                {
                    mess = "Ошибка в конце выражения";
                    token = null;
                }
                else mess = "Недопустимое использование лексемы";
            }
            if (token != null) AddError(mess, token);
            else AddError(mess, null, line, charPositionInLine);
        }
    }
}