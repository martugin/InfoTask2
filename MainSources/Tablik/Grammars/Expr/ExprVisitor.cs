using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using CompileLibrary;
using P = Tablik.ExprParser;

namespace Tablik
{
    internal class ExprVisitor : ExprBaseVisitor<IExprNode>
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
            return tree == null ? null : Visit(tree);
        }
        internal IExprNode[] GoList(IParseTree tree)
        {
            return tree == null ? null : ((ListExprNode)Visit(tree)).Nodes;
        }

        //Выражения без значения
        

        //Выражения со значением
        public override IExprNode VisitExprCons(P.ExprConsContext context)
        {
            return Go(context.cons());
        }

        public override IExprNode VisitExprSignal(P.ExprSignalContext context)
        {
            return new SignalNode(_keeper, context.SIGNAL());
        }

        public override IExprNode VisitExprParen(P.ExprParenContext context)
        {
            return Go(context.expr());
        }

        public override IExprNode VisitExprIf(P.ExprIfContext context)
        {
            _keeper.CheckParenths(context);
            return new IfNode(_keeper, context.IF(), context.expr().Select(Go), context.valueProg().Select(Go));
        }

        public override IExprNode VisitExprAbsolute(P.ExprAbsoluteContext context)
        {
            _keeper.CheckParenths(context);
            throw new NotImplementedException();
        }

        public override IExprNode VisitExprGraphic(P.ExprGraphicContext context)
        {
            _keeper.CheckParenths(context);
            return new GraficNode(_keeper, context.IDENT(), GoList(context.pars()));
        }

        public override IExprNode VisitExprTabl(P.ExprTablContext context)
        {
            _keeper.CheckParenths(context);
            throw new NotImplementedException();
        }

        public override IExprNode VisitExprTablC(P.ExprTablCContext context)
        {
            _keeper.CheckParenths(context);
            throw new NotImplementedException();
        }

        public override IExprNode VisitExprIdent(P.ExprIdentContext context)
        {
            var text = context.IDENT().Symbol.Text;
            if (_keeper.Param.Vars.ContainsKey(text))
                return new VarNode(context.IDENT(), _keeper.Param.Vars[text]);
            if (_keeper.Module.Params.ContainsKey(text))
                return new ParamNode(_keeper, context.IDENT(), _keeper.Module.Params[text]);
            if (_keeper.Param.Params.ContainsKey(text))
                return new ParamNode(_keeper, context.IDENT(), _keeper.Param.Params[text]);
            if (_keeper.FunsChecker.Funs.ContainsKey(text))
                return new FunNode(_keeper, context.IDENT());
            _keeper.AddError("Неизвестный идентификатор", context.IDENT());
            return new ErrorNode(context.IDENT());
        }

        public override IExprNode VisitExprFun(P.ExprFunContext context)
        {
            _keeper.CheckParenths(context);
            var text = context.IDENT().Symbol.Text;
            var pars = GoList(context.pars());
            if (_keeper.Module.Params.ContainsKey(text))
                return new ParamNode(_keeper, context.IDENT(), _keeper.Module.Params[text], pars);
            if (_keeper.Param.Params.ContainsKey(text))
                return new ParamNode(_keeper, context.IDENT(), _keeper.Param.Params[text], pars);
            if (_keeper.FunsChecker.Funs.ContainsKey(text))
                return new FunNode(_keeper, context.IDENT(), pars);
            _keeper.AddError("Неизвестная функция", context.IDENT());
            return new ErrorNode(context.IDENT());
        }

        public override IExprNode VisitExprMet(P.ExprMetContext context)
        {
            return new MetNode(_keeper, context.IDENT(), Go(context.expr()));
        }

        public override IExprNode VisitExprMetFun(P.ExprMetFunContext context)
        {
            _keeper.CheckParenths(context);
            return new MetNode(_keeper, context.IDENT(), Go(context.expr()), GoList(context.pars()));
        }

        public override IExprNode VisitExprMetSignal(P.ExprMetSignalContext context)
        {
            return new MetSignalNode(_keeper, context.SIGNAL(), Go(context.expr()));
        }

        public override IExprNode VisitExprUnary(P.ExprUnaryContext context)
        {
            return new FunNode(_keeper, context.MINUS(), Go(context.expr()));
        }

        public override IExprNode VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new FunNode(_keeper, fun, Go(context.expr(0)), Go(context.expr(1)));
        }

        //Список аргументов функции
        public override IExprNode VisitParamsList(P.ParamsListContext context)
        {
            return new ListExprNode(context.expr().Select(Go));
        }
        public override IExprNode VisitParamsEmpty(P.ParamsEmptyContext context)
        {
            return new ListExprNode();
        }

        //Тип данных переменной
        public override IExprNode VisitTypeSimple(P.TypeSimpleContext context)
        {
            return new DataTypeNode(context.DATATYPE());
        }

        public override IExprNode VisitTypeArray(P.TypeArrayContext context)
        {
            return new ArrayTypeNode(context.ARRAY(), context.DATATYPE());
        }

        public override IExprNode VisitTypeSignal(P.TypeSignalContext context)
        {
            return new SignalTypeNode(_keeper, context.SIGNAL());
        }

        public override IExprNode VisitTypeParam(P.TypeParamContext context)
        {
            return new ParamTypeNode(_keeper, context.IDENT());
        }

        //Константы
        public override IExprNode VisitConsInt(P.ConsIntContext context)
        {
            return (IExprNode)_keeper.GetIntConst(context.INT());
        }

        public override IExprNode VisitConsReal(P.ConsRealContext context)
        {
            return (IExprNode)_keeper.GetRealConst(context.REAL());
        }

        public override IExprNode VisitConsString(P.ConsStringContext context)
        {
            return (IExprNode)_keeper.GetStringConst(context.STRING(), true);
        }

        public override IExprNode VisitConsTime(P.ConsTimeContext context)
        {
            return (IExprNode)_keeper.GetTimeConst(context.TIME());
        }
    }
}