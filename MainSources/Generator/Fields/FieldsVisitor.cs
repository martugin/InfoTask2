﻿using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using Generator.Fields;
using P = Generator.Fields.FieldsParsemes;

namespace Generator
{
    internal class FieldsVisitor : FieldsParsemesBaseVisitor<Node> 
    {
        public FieldsVisitor(GenKeeper keeper)
        {
            _keeper = keeper;
        }
        
        //Формирование строки ошибки
        private readonly GenKeeper _keeper;

        //Обход дерева разбора, возвращаются разные типы вершин
        public Node Go(IParseTree tree)
        {
            if (tree == null) return null;
            return Visit(tree);
        }
        public INodeExpr GoExpr(IParseTree tree)
        {
            return (INodeExpr)Go(tree);
        }
        public INodeVoid GoVoid(IParseTree tree)
        {
            return (INodeVoid)Go(tree);
        }
        public NodeList GoList(IParseTree tree)
        {
            return (NodeList)Go(tree);
        }

        public override Node VisitFieldGen(P.FieldGenContext context)
        {
            return Go(context.textGen());
        }

        public override Node VisitTextGen(P.TextGenContext context)
        {
            return new NodeTextList(context.element().Select(Go));
        }

        public override Node VisitElementText(P.ElementTextContext context)
        {
            return _keeper.GetStringConst((ITerminalNode)context.children[0], false);
        }

        public override Node VisitElementVoid(P.ElementVoidContext context)
        {
            _keeper.CheckSquareParenths(context);
            return Go(context.voidProg());
        }

        public override Node VisitElementValue(P.ElementValueContext context)
        {
            _keeper.CheckSquareParenths(context);
            return Go(context.valueProg());
        }

        public override Node VisitVoidProg(P.VoidProgContext context)
        {
            return new NodeVoidProg(context.voidExpr().Select(GoVoid).ToArray()); 
        }
        
        public override Node VisitValueProg(P.ValueProgContext context)
        {
            return new NodeValueProg(new NodeVoidProg(context.voidExpr().Select(GoVoid).ToArray()), 
                                                    GoExpr(context.expr()));
        }

        //Выражения без значения

        public override Node VisitVoidExprVar(P.VoidExprVarContext context)
        {
            return new NodeVarSet(_keeper, context.IDENT(), GoExpr(context.expr()));
        }

        public override Node VisitVoidExprIf(P.VoidExprIfContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeIfVoid(_keeper, context.IF(), 
                                               context.expr().Select(GoExpr).ToList(),
                                               context.voidProg().Select(GoVoid).ToList());
        }

        public override Node VisitVoidExprWhile(P.VoidExprWhileContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeWhileVoid(_keeper, context.WHILE(),
                                                    GoExpr(context.expr()),
                                                    GoVoid(context.voidProg()));
        }

        public override Node VisitVoidExprOver(P.VoidExprOverContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeOverVoid(_keeper, context.OVERTABL(), GoVoid(context.voidProg()));
        }

        public override Node VisitVoidExprSub(P.VoidExprSubContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeSubVoid(_keeper, context.SUBTABL(),  
                                                  context.expr() == null ? null : GoExpr(context.expr()),
                                                  GoVoid(context.voidProg()));
        }

        //Выражения со значением

        public override Node VisitExprCons(P.ExprConsContext context)
        {
            return Go(context.cons());
        }

        public override Node VisitExprParen(P.ExprParenContext context)
        {
            _keeper.CheckParenths(context);
            return Go(context.expr());
        }

        public override Node VisitExprIf(P.ExprIfContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeIf(_keeper, context.IF(),
                                         context.expr().Select(GoExpr).ToList(),
                                         context.valueProg().Select(GoExpr).ToList());
        }

        public override Node VisitExprWhile(P.ExprWhileContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeWhile(_keeper, context.WHILE(), 
                                              GoExpr(context.expr(0)), 
                                              GoExpr(context.valueProg()), 
                                              GoExpr(context.expr(1)));
        }

        public override Node VisitExprOver(P.ExprOverContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeOver(_keeper, context.OVERTABL(), GoExpr(context.valueProg()));
        }

        public override Node VisitExprSub(P.ExprSubContext context)
        {
            _keeper.CheckParenths(context);
            return new NodeSub(_keeper, (ITerminalNode) context.children[0], context.valueProg().Select(GoExpr));
        }

        public override Node VisitExprIdent(P.ExprIdentContext context)
        {
            if (_keeper.Vars.ContainsKey(context.IDENT().Symbol.Text))
                return new NodeVar(_keeper, context.IDENT());
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
            return new NodeFun(_keeper, (ITerminalNode)context.children[0], GoExpr(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new NodeFun(_keeper, fun, context.expr().Select(GoExpr).ToArray());
        }

        public override Node VisitExprTextGen(P.ExprTextGenContext context)
        {
            return Go(context.textGen());
        }

        public override Node VisitParamsList(P.ParamsListContext context)
        {
            return new NodeList(context.expr().Select(Go));
        }

        public override Node VisitParamsEmpty(P.ParamsEmptyContext context)
        {
            return new NodeList(new Node[0]);
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