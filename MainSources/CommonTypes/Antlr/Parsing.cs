using System;
using System.IO;
using Antlr4.Runtime;

namespace CommonTypes
{
    //Базовый класс для всех разборщиков ANTLR
    public abstract class Parsing
    {
        protected Parsing(string fieldName, //Имя разбираемого поля (для сообщений об ошибках)
                                   string fieldValue) //Разбираемое выражение
        {
            FieldName = fieldName;
            var reader = new StringReader(fieldValue);
            var input = new AntlrInputStream(reader.ReadToEnd());
            var lexer = GetLexer(input);
            var errorLexerListener = new LexerErrorListener(fieldName, fieldValue);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(errorLexerListener);
            var tokens = new CommonTokenStream(lexer);
            var parser = GetParser(tokens);
            var errorParserListener = new ParserErrorListener(fieldName);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorParserListener);
            var keeper = new ParsingKeeper(fieldName);
            ResultTree = RunVisitor(parser, keeper);
            ErrMess = errorLexerListener.ErrMess + errorParserListener.ErrMess + keeper.ErrMess;
        }

        //Имя разбираемого поля
        protected string FieldName { get; private set; }

        //Возвращаемое дерево и строка с ошибками разбора
        public Node ResultTree { get; private set; }
        public string ErrMess { get; private set; }

        //Строка с результатами парсинга для тестов
        public string ToTestString()
        {
            return (ResultTree == null ? "" : ResultTree.ToTestString()) + Environment.NewLine + ErrMess;
        }

        //Создать лексер
        protected abstract Lexer GetLexer(ICharStream input);
        //Создать парсер
        protected abstract Parser GetParser(ITokenStream tokens);
        //Создать и запустить визитор, возвращает дерево разбора
        protected abstract Node RunVisitor(Parser parser, ParsingKeeper keeper);
    }
}