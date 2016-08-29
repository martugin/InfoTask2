using Antlr4.Runtime;
using CommonTypes;
using Generator.Fields;
using Generator.Grammars;

namespace Generator
{
    internal class FieldsParsing : RuleParsing
    {
        public FieldsParsing(string fieldName, string fieldValue)
            : base(fieldName, fieldValue) { }

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
            var v = new FieldsVisitor(keeper);
            return v.Go(p.fieldGen());
        }
    }
}