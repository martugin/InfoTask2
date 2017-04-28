using Antlr4.Runtime;
using CompileLibrary;
using Generator.Fields;

namespace Generator
{
    //Класс, запускающий разбор для Fields
    internal class FieldsParsing : Parsing
    {
        public FieldsParsing(GenKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue) { }

        protected override Lexer GetLexer(ICharStream input)
        {
            return new FieldsLexemes(input);
        }

        protected override Parser GetParser(ITokenStream tokens)
        {
            return new FieldsParsemes(tokens);
        }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            var p = (FieldsParsemes)parser;
            var v = new FieldsVisitor((GenKeeper)keeper);
            return v.Go(p.fieldGen());
        }
    }
}