using System;
using Antlr4.Runtime;
using CompileLibrary;
using Generator.Grammars;

namespace Generator
{
    //Класс, запускающий разбор для GenRule
    internal class RuleParsing : Parsing
    {
        public RuleParsing(GenKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue + Environment.NewLine) { }

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
            return new RuleVisitor(keeper).Go(((RuleParser)parser).tablGen());
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //Класс, запускающий разбор для GenRule подтаблицы
    internal class SubRuleParsing : RuleParsing
    {
        public SubRuleParsing(GenKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue + Environment.NewLine) { }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            return new RuleVisitor(keeper).Go(((RuleParser)parser).subTablGen());
        }
    }
}