using System;
using CompileLibrary;
using Antlr4.Runtime;

namespace Tablik
{
    //Класс, запускающий разбор для списка входов
    public class InputsParsing : Parsing
    {
        public InputsParsing(ParsingKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue + Environment.NewLine)
        {
        }

        protected override Lexer GetLexer(ICharStream input)
        {
            return new InputsLexer(input);
        }

        protected override Parser GetParser(ITokenStream tokens)
        {
            return new InputsParser(tokens);
        }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            return new InputsVisitor(keeper).Go(((InputsParser)parser).prog());
        }
    }
}