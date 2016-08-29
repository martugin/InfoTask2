using Antlr4.Runtime;
using CommonTypes;
using Generator.Grammars;

namespace Generator
{
    internal class RuleParsing : Parsing
    {
        public RuleParsing(string fieldName, string fieldValue, bool isSubTabl = false)
            : base(fieldName, fieldValue)
        {
            _isSubTabl = isSubTabl;
        }

        //Используется для подтаблицы
        private readonly bool _isSubTabl;

        protected override Lexer GetLexer(ICharStream input)
        {
            return new RuleLexer(input);
        }

        protected override Parser GetParser(ITokenStream tokens)
        {
            return new RuleParser(tokens);
        }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            var p = (RuleParser)parser;
            var v = new RuleVisitor(keeper);
            return _isSubTabl ? v.Go(p.subTablGen()) : v.Go(p.tablGen());
        }
    }
}