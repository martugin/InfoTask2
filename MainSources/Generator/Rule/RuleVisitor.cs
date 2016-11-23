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
        public INodeExpr GoExpr(IParseTree tree)
        {
            return (INodeExpr)Go(tree);
        }
        public NodeList GoList(IParseTree tree)
        {
            return (NodeList)Go(tree);
        }

        //Обход разных типов узлов

        public override Node VisitSubTablGenTabl(P.SubTablGenTablContext context)
        {
            return Go(context.tablGen());
        }
       
        public override Node VisitSubTablGenQuery(P.SubTablGenQueryContext context)
        {
            return Go(context.query());
        }

        public override Node VisitTablGenSimple(P.TablGenSimpleContext context)
        {
            return Go(context.tabl());
        }

        public override Node VisitTablGenQuery(P.TablGenQueryContext context)
        {
            var tabl = (NodeRTabl)Go(context.tabl());
            tabl.ChildNode = (INodeRQuery)Go(context.query());
            return tabl;
        }

        public override Node VisitQuerySubTabl(P.QuerySubTablContext context)
        {
            return Go(context.subTabl());
        }

        public override Node VisitQueryGroup(P.QueryGroupContext context)
        {
            return Go(context.rowGroup());
        }

        public override Node VisitQuerySubTablGroup(P.QuerySubTablGroupContext context)
        {
            var sub = (NodeRSub)Go(context.subTabl());
            sub.ChildNode = (INodeRQuery)Go(context.query());
            return sub;
        }
        
        public override Node VisitTablIdent(P.TablIdentContext context)
        {
            return new NodeRTabl(_keeper, context.IDENT(), null);
        }

        public override Node VisitTablCond(P.TablCondContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeRTabl(_keeper, context.IDENT(), GoExpr(context.expr()));
        }
        
        public override Node VisitSubTablIdent(P.SubTablIdentContext context)
        {
            return new NodeRSub(_keeper, context.SUBTABL(), null);
        }

        public override Node VisitSubTablCond(P.SubTablCondContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeRSub(_keeper, context.SUBTABL(), GoExpr(context.expr()));
        }

        public override Node VisitGroupSimple(P.GroupSimpleContext context)
        {
            return new NodeRGroup(_keeper, context.ROWGROUP(), null);
        }

        public override Node VisitGroupIdent(P.GroupIdentContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeRGroup(_keeper, context.ROWGROUP(), 
                                                new NodeList(context.IDENT().Select(field => _keeper.GetStringConst(field, false))));
        }

        //Выражения

        public override Node VisitExprCons(P.ExprConsContext context)
        {
            return Go(context.cons());
        }

        public override Node VisitExprParen(P.ExprParenContext context)
        {
            _keeper.CheckParenths(context);
            return Go(context.expr());
        }

        public override Node VisitExprIdent(P.ExprIdentContext context)
        {
            return new NodeField(_keeper, context.IDENT());
        }

        public override Node VisitExprFunConst(P.ExprFunConstContext context)
        {
            return new NodeFun(_keeper, context.FUNCONST());
        }

        public override Node VisitExprFun(P.ExprFunContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeFun(_keeper, context.IDENT(), GoList(context.pars()));
        }

        public override Node VisitExprUnary(P.ExprUnaryContext context)
        {
            var fun = (ITerminalNode)context.children[0];
            return new NodeFun(_keeper, fun, GoExpr(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new NodeFun(_keeper, fun, GoExpr(context.expr(0)), GoExpr(context.expr(1)));
        }
        
        public override Node VisitParamsList(P.ParamsListContext context)
        {
            return new NodeList(context.expr().Select(Visit).ToList());
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
            return _keeper.GetStringConst(context.STRING(), true);
        }

        public override Node VisitConsTime(P.ConsTimeContext context)
        {
            return _keeper.GetTimeConst(context.TIME());
        }
    }
}