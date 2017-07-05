﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CompileLibrary;
using P = Tablik.ExprParser;

namespace Tablik
{
    internal class ExprVisitor : ExprBaseVisitor<Node>
    {
        internal ExprVisitor(TablikKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly TablikKeeper _keeper;

        //Обход дерева разбора
        internal IExprNode Go(IParseTree tree)
        {
            if (tree == null) return null;
            return (IExprNode)Visit(tree);
        }
        
        //Выражения без значения
        

        //Выражения со значением
        public override Node VisitExprCons(P.ExprConsContext context)
        {
            return (Node)Go(context.cons());
        }

        public override Node VisitExprSignal(P.ExprSignalContext context)
        {
            return new SignalNode(_keeper, context.SIGNAL());
        }

        public override Node VisitExprParen(P.ExprParenContext context)
        {
            return (Node)Go(context.expr());
        }

        public override Node VisitExprIf(P.ExprIfContext context)
        {
            return new IfNode(_keeper, context.IF(), context.expr().Select(Go), context.valueProg().Select(Go));
        }

        public override Node VisitExprAbsolute(P.ExprAbsoluteContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprGraphic(P.ExprGraphicContext context)
        {
            return new GraficNode(_keeper, 
                                               context.IDENT(), 
                                               (ListNode<IExprNode>)Go(context.pars()));
        }

        public override Node VisitExprTabl(P.ExprTablContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprTablC(P.ExprTablCContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprFun(P.ExprFunContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprMet(P.ExprMetContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprMetSignal(P.ExprMetSignalContext context)
        {
            throw new NotImplementedException();
        }

        public override Node VisitExprUnary(P.ExprUnaryContext context)
        {
            return new FunNode(_keeper, context.MINUS(), Go(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new FunNode(_keeper, fun, Go(context.expr(0)), Go(context.expr(1)));
        }

        //Список аргументов функции
        public override Node VisitParamsList(P.ParamsListContext context)
        {
            return new ListNode<IExprNode>(context.expr().Select(Go));
        }
        public override Node VisitParamsEmpty(P.ParamsEmptyContext context)
        {
            return new ListNode<IExprNode>(new List<IExprNode>());
        }

        //Тип данных переменной
        public override Node VisitTypeSimple(P.TypeSimpleContext context)
        {
            return new DataTypeNode(context.DATATYPE());
        }

        public override Node VisitTypeArray(P.TypeArrayContext context)
        {
            return new ArrayTypeNode(context.ARRAY(), context.DATATYPE());
        }

        public override Node VisitTypeSignal(P.TypeSignalContext context)
        {
            return new SignalTypeNode(_keeper, context.SIGNAL());
        }

        public override Node VisitTypeParam(P.TypeParamContext context)
        {
            return new ParamTypeNode(_keeper, context.IDENT());
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