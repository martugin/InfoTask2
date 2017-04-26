using Antlr4.Runtime;

namespace CompileLibrary
{
    //Прослушиватель ошибок лексера
    public class LexerErrorListener : IAntlrErrorListener<int>
    {
        public LexerErrorListener(ParsingKeeper keeper, string fieldValue)
        {
            _fieldValue = fieldValue;
            _keeper = keeper;
        }

        //Разбираемая строка
        private readonly string _fieldValue;
        //Накопитель ошибок
        private readonly ParsingKeeper _keeper;

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var lexer = (Lexer)recognizer;
            const string mess = "Недопустимая последовательность символов";
            string last = "";
            if (lexer.CharIndex < _fieldValue.Length)
                last += _fieldValue[lexer.CharIndex];
            if (lexer.Token != null)
            {
                var t = lexer.Token;
                _keeper.AddError(mess, t.Text.Trim() + last, t.Line, t.Column, t);
            }
            else _keeper.AddError(mess, lexer.Text.Trim() + last, line, charPositionInLine);
        }
    }

    //-------------------------------------------------------------------------------------------------
    //Прослушиватель ошибок парсера
    public class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        public ParserErrorListener(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        //Накопитель ошибок
        private readonly ParsingKeeper _keeper;

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
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
            if (token != null) _keeper.AddError(mess, token);
            else _keeper.AddError(mess, null, line, charPositionInLine);
        }
    }
}