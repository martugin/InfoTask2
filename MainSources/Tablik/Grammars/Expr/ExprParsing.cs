using System;
using Antlr4.Runtime;
using CompileLibrary;

namespace Tablik
{
    public class ExprParsing : Parsing
    {
        public ExprParsing(ParsingKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue + Environment.NewLine)
        {
        }

        protected override Lexer GetLexer(ICharStream input)
        {
            return new ExprLexer(input);
        }

        protected override Parser GetParser(ITokenStream tokens)
        {
            return new ExprParser(tokens);
        }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            return (Node)new ExprVisitor((TablikKeeper)keeper).Go(((ExprParser)parser).prog());
        }
    }
}