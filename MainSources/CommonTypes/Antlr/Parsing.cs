using System.IO;
using Antlr4.Runtime;

namespace CommonTypes
{
    //Базовый класс для всех разборщиков ANTLR
    public abstract class Parsing
    {
        protected Parsing(ParsingKeeper keeper, //Накопитель ошибок
                                   string fieldName, //Имя разбираемого поля (для сообщений об ошибках)
                                   string fieldValue) //Разбираемое выражение
        {
            _keeper = keeper;
            keeper.SetFieldName(fieldName);
            var reader = new StringReader(fieldValue);
            var input = new AntlrInputStream(reader.ReadToEnd());
            var lexer = GetLexer(input);
            var errorLexerListener = new LexerErrorListener(keeper, fieldValue);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(errorLexerListener);
            var tokens = new CommonTokenStream(lexer);
            var parser = GetParser(tokens);
            var errorParserListener = new ParserErrorListener(keeper);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorParserListener);
            ResultTree = RunVisitor(parser, keeper);
        }

        //Возвращаемое дерево и строка с ошибками разбора
        public Node ResultTree { get; private set; }
        
        //Накопление ошибки
        private readonly ParsingKeeper _keeper;
        //Строка с результатами парсинга для тестов
        public string ToTestString()
        {
            return (ResultTree == null ? "" : ResultTree.ToTestString()) + (_keeper.ErrMess == "" ? "" : (" " + _keeper.ErrMess));
        }

        //Создать лексер
        protected abstract Lexer GetLexer(ICharStream input);
        //Создать парсер
        protected abstract Parser GetParser(ITokenStream tokens);
        //Создать и запустить визитор, возвращает дерево разбора
        protected abstract Node RunVisitor(Parser parser, ParsingKeeper keeper);
    }
}