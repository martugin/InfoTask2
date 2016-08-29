using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using Generator.Grammars;
using P = Generator.Grammars.RuleParser;

namespace Generator
{
    internal class RuleVisitor : RuleBaseVisitor<Node>
    {
        public RuleVisitor(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly ParsingKeeper _keeper;

        //Обход дерева разбора
        public Node Go(IParseTree tree)
        {
            if (tree == null) return null;
            return Visit(tree);
        }

        //Обход разных типов узлов

        public override Node VisitTablGenOver(P.TablGenOverContext context)
        {
            return new NodeROverTabl(context.IDENT(), context.OVERTABL());
        }

        public override Node VisitTablGenSimple(P.TablGenSimpleContext context)
        {
            var tabl = (NodeIter)Go(context.tabl());
            foreach (var s in context.subTabl())
            {
                var sub = (NodeRSubTabl)Go(s);
                sub.Parent = tabl;
                tabl = sub;
            }
            return tabl;
        }

        public override Node VisitSubTablGen(P.SubTablGenContext context)
        {
            NodeRSubTabl tabl = null;
            foreach (var s in context.subTabl())
            {
                var sub = (NodeRSubTabl)Go(s);
                sub.Parent = tabl;
                tabl = sub;
            }
            return tabl;
        }

        public override Node VisitTablIdent(P.TablIdentContext context)
        {
            return new NodeRTabl(context.IDENT());
        }

        public override Node VisitTablCond(P.TablCondContext context)
        {
            return new NodeRTabl(context.IDENT(), (NodeExpr)Go(context.expr()));
        }

        public override Node VisitTablParenLost(P.TablParenLostContext context)
        {
            _keeper.AddError("Не закрытая скобка", context.LPAREN());
            return new NodeRTabl(context.IDENT(), (NodeExpr)Go(context.expr()));
        }

        public override Node VisitTablParenExtra(P.TablParenExtraContext context)
        {
            _keeper.AddError("Лишняя закрывающаяся скобка", context.RPAREN());
            return new NodeRTabl(context.IDENT(), (NodeExpr)Go(context.expr()));
        }

        public override Node VisitSubTablIdent(P.SubTablIdentContext context)
        {
            return new NodeRTabl(context.SUBTABL());
        }

        public override Node VisitSubTablCond(P.SubTablCondContext context)
        {
            return new NodeRTabl(context.SUBTABL(), (NodeExpr)Go(context.expr()));
        }

        public override Node VisitSubTablParenLost(P.SubTablParenLostContext context)
        {
            _keeper.AddError("Не закрытая скобка", context.LPAREN());
            return new NodeRTabl(context.SUBTABL(), (NodeExpr)Go(context.expr()));
        }

        public override Node VisitSubTablParenExtra(P.SubTablParenExtraContext context)
        {
            _keeper.AddError("Лишняя закрывающаяся скобка", context.RPAREN());
            return new NodeRTabl(context.SUBTABL(), (NodeExpr)Go(context.expr()));
        }

        //Выражения

        public override Node VisitExprCons(P.ExprConsContext context)
        {
            return Go(context.cons());
        }

        public override Node VisitExprParen(P.ExprParenContext context)
        {
            return Go(context.expr());
        }

        public override Node VisitExprIdent(P.ExprIdentContext context)
        {
            return new NodeField(context.IDENT());
        }

        public override Node VisitExprFun(P.ExprFunContext context)
        {
            return new NodeFun(context.IDENT(), (NodeList)Go(context.pars()));
        }

        public override Node VisitExprUnary(P.ExprUnaryContext context)
        {
            var fun = (ITerminalNode)context.children[0];
            return new NodeFun(fun, (NodeExpr)Go(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new NodeFun(fun, (NodeExpr)Go(context.expr(0)), (NodeExpr)Go(context.expr(1)));
        }

        public override Node VisitExprFunParenLost(P.ExprFunParenLostContext context)
        {
            _keeper.AddError("Не закрытая скобка", context.LPAREN());
            return new NodeFun(context.IDENT(), (NodeList)Go(context.pars()));
        }

        public override Node VisitExprFunParenExtra(P.ExprFunParenExtraContext context)
        {
            _keeper.AddError("Лишняя закрывающаяся скобка", context.RPAREN());
            return new NodeFun(context.IDENT(), (NodeList)Go(context.pars()));
        }

        public override Node VisitParamsList(P.ParamsListContext context)
        {
            return new NodeList(context.expr().Select(Visit));
        }

        public override Node VisitParamsEmpty(P.ParamsEmptyContext context)
        {
            return new NodeList(new List<Node>());
        }

        //Константы

        public override Node VisitConsInt(P.ConsIntContext context)
        {
            return _keeper.GetIntConst(context.INT());
        }

        public override Node VisitConsReal(P.ConsRealContext context)
        {
            return _keeper.GetRealConst(context.REAL());
        }

        public override Node VisitConsString(P.ConsStringContext context)
        {
            return _keeper.GetStringConst(context.STRING());
        }
    }
}