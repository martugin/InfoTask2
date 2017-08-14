﻿using System;
using Antlr4.Runtime;
using CompileLibrary;

namespace Calculation
{
    public class CalcExprParsing : Parsing
    {
        public CalcExprParsing(ParsingKeeper keeper, string fieldName, string fieldValue)
            : base(keeper, fieldName, fieldValue + Environment.NewLine)
        {
        }

        protected override Lexer GetLexer(ICharStream input)
        {
            return new CalcExprLexer(input);
        }

        protected override Parser GetParser(ITokenStream tokens)
        {
            return new CalcExprParser(tokens);
        }

        protected override Node RunVisitor(Parser parser, ParsingKeeper keeper)
        {
            return (Node)new CalcExprVisitor(keeper).Go(((CalcExprParser)parser).prog());
        }
    }
}