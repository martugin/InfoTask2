using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using System.Collections.Generic;
using P = Calculation.CalcExprParser;

namespace Calculation
{
    public class CalcExprVisitor : CalcExprBaseVisitor<ICalcNode>
    {
        internal CalcExprVisitor(CalcKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly CalcKeeper _keeper;

        //Расчетный параметр
        private CalcParam Param { get { return _keeper.Param; }}
        //Модуль
        private CalcModule Module { get { return Param.Module; }}

        //Обход дерева разбора
        internal ICalcNode Go(IParseTree tree)
        {
            return tree == null ? null : Visit(tree);
        }

        internal CalcNodeInfo GoInfo(IParseTree tree)
        {
            return (CalcNodeInfo)Go(tree);
        }

        public override ICalcNode VisitProg(P.ProgContext context)
        {
            Go(context.vars());
            return Go(context.expr());
        }

        public override ICalcNode VisitInputs(P.InputsContext context)
        {
            foreach (var v in context.IDENT().Select(x => x.Symbol.Text))
            {
                Param.Inputs.Add(v);
                Param.Vars.Add(v);
            }
            return null;
        }

        public override ICalcNode VisitVars(P.VarsContext context)
        {
            foreach (var v in context.IDENT().Select(x => x.Symbol.Text))
                Param.Vars.Add(v);
            return null;
        }

        //Стек узлов со значением
        private Stack<ICalcNode> _stack;
        //Стек узлов - списков операций
        private Stack<OperatorNode> _stackOp;

        public override ICalcNode VisitExprs(P.ExprsContext context)
        {
            _stack = new Stack<ICalcNode>();
            _stackOp = new Stack<OperatorNode>();
            _stackOp.Push(new OperatorNode());
            foreach (var expr in context.expr())
                Go(expr);
            return _stackOp.Pop();
        }

        public override ICalcNode VisitExprBool(P.ExprBoolContext context)
        {
            var node = (ICalcNode) _keeper.GetIntConst(context.INT());
            _stack.Push(node);
            return node;
        }

        public override ICalcNode VisitExprInt(P.ExprIntContext context)
        {
            var node = (ICalcNode)_keeper.GetIntConst(context.INT());
            _stack.Push(node);
            return node;
        }

        public override ICalcNode VisitExprReal(P.ExprRealContext context)
        {
            var node = (ICalcNode)_keeper.GetRealConst(context.REAL());
            _stack.Push(node);
            return node;
        }

        public override ICalcNode VisitExprTime(P.ExprTimeContext context)
        {
            var node = (ICalcNode)_keeper.GetTimeConst(context.TIME());
            _stack.Push(node);
            return node;
        }

        public override ICalcNode VisitExprString(P.ExprStringContext context)
        {
            var node = (ICalcNode)_keeper.GetStringConst(context.STRING(), true);
            _stack.Push(node);
            return node;
        }

        public override ICalcNode VisitExprVoid(P.ExprVoidContext context)
        {
            return new VoidNode();
        }

        public override ICalcNode VisitExprParam(P.ExprParamContext context)
        {
            var info = GoInfo(context.info());
            CalcModule mod = null;
            if (Module.CalcParams.ContainsKey(info.Code))
                mod = Module;
            else foreach (var m in Module.LinkedModules)
                if (m.CalcParams.ContainsKey(info.Code))
                    mod = m;
            //return new ParamNode(new CalcParamInstance(mod.CalcParams[info.Code]));
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprSubParam(P.ExprSubParamContext context)
        {
            var info = GoInfo(context.info());
            //return new ParamNode(new CalcParamInstance(Param.CalcParams[info.Code]));
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprVar(P.ExprVarContext context)
        {
            //return new VarNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprAssign(P.ExprAssignContext context)
        {
            //return new AssignNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprFun(P.ExprFunContext context)
        {
            //return new FunNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprGrafic(P.ExprGraficContext context)
        {
            //return new GraficNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprSignal(P.ExprSignalContext context)
        {
            //return new SignalNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprObject(P.ExprObjectContext context)
        {
            //return new ObjectNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprMetSignal(P.ExprMetSignalContext context)
        {
            //return new MetSignalNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprMet(P.ExprMetContext context)
        {
            //return new MetNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprOwner(P.ExprOwnerContext context)
        {
            //return new ParamNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprParamProp(P.ExprParamPropContext context)
        {
            //return new ParamPropNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprObjectProp(P.ExprObjectPropContext context)
        {
            //return new ObjectPropNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprSubParams(P.ExprSubParamsContext context)
        {
            //return new SubParamsNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprPrev(P.ExprPrevContext context)
        {
            //return new PrevNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprIf(P.ExprIfContext context)
        {
            //return new IfNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprWhile(P.ExprWhileContext context)
        {
            //return new WhileNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprTablField(P.ExprTablFieldContext context)
        {
            //return new TablNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprTabl(P.ExprTablContext context)
        {
            //return new TablNode();
            throw new NotImplementedException();
        }

        public override ICalcNode VisitExprError(P.ExprErrorContext context)
        {
            throw new NotImplementedException();
        }

        //info
        public override ICalcNode VisitInfoSimple(P.InfoSimpleContext context)
        {
            return new CalcNodeInfo(context.IDENT().Symbol.Text, 0, _stack);
        }

        public override ICalcNode VisitInfoArgs(P.InfoArgsContext context)
        {
            return new CalcNodeInfo(context.IDENT().Symbol.Text, context.INT().Symbol.Text.ToInt(), _stack);
        }

        public override ICalcNode VisitInfoSignal(P.InfoSignalContext context)
        {
            var text = context.SIGNAL().Symbol.Text;
            return new CalcNodeInfo(text.Substring(1, text.Length - 2), 0, _stack);
        }

        public override ICalcNode VisitInfoSignalArgs(P.InfoSignalArgsContext context)
        {
            var text = context.SIGNAL().Symbol.Text;
            return new CalcNodeInfo(text.Substring(1, text.Length - 2), context.INT().Symbol.Text.ToInt(), _stack);
        }
    }
}