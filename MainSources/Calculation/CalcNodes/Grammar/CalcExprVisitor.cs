using System.Linq;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CompileLibrary;
using System.Collections.Generic;
using P = Calculation.CalcExprParser;

namespace Calculation
{
    public class CalcExprVisitor : CalcExprBaseVisitor<ICalcNode>
    {
        public CalcExprVisitor(ParsingKeeper keeper, CalcParam param)
        {
            _keeper = keeper;
            _param = param;
        }

        //Формирование строки ошибки
        private readonly ParsingKeeper _keeper;

        //Расчетный параметр
        private readonly CalcParam _param;
        //Модуль
        private CalcModule Module { get { return _param.Module; }}

        //Обход дерева разбора
        internal ICalcNode Go(IParseTree tree)
        {
            return tree == null ? null : Visit(tree);
        }

        public override ICalcNode VisitProg(P.ProgContext context)
        {
            Go(context.vars());
            return Go(context.expr());
        }

        public override ICalcNode VisitVars(P.VarsContext context)
        {
            if (context.var() != null)
                context.var().Select(Go);
            return null;
        }

        public override ICalcNode VisitVar(P.VarContext context)
        {
            var code = context.IDENT().Symbol.Text;
            _param.Vars.Add(code, new CalcVar(code));
            return null;
        }

        public override ICalcNode VisitTypeF(P.TypeFContext context)
        {
            return null;
        }

        //Стек для формирования дерева вершин
        private readonly Stack<ICalcNode> _stack = new Stack<ICalcNode>();

        public override ICalcNode VisitExprs(P.ExprsContext context)
        {
            var nodes = context.expr().Select(Go);
            foreach (var node in nodes)
            {
                
            }
        }

        public override ICalcNode VisitExprBool(P.ExprBoolContext context)
        {

        }

        public override ICalcNode VisitExprInt(P.ExprIntContext context)
        {

        }

        public override ICalcNode VisitExprReal(P.ExprRealContext context)
        {

        }

        public override ICalcNode VisitExprTime(P.ExprTimeContext context)
        {

        }

        public override ICalcNode VisitExprString(P.ExprStringContext context)
        {

        }

        public override ICalcNode VisitExprTablField(P.ExprTablFieldContext context)
        {

        }

        public override ICalcNode VisitExprTabl(P.ExprTablContext context)
        {

        }

        public override ICalcNode VisitExprError(P.ExprErrorContext context)
        {

        }


        public override ICalcNode VisitExprVoid(P.ExprVoidContext context)
        {

        }

        public override ICalcNode VisitExprParam(P.ExprParamContext context)
        {

        }

        public override ICalcNode VisitExprSubParam(P.ExprSubParamContext context)
        {

        }

        public override ICalcNode VisitExprVar(P.ExprVarContext context)
        {

        }

        public override ICalcNode VisitExprAssign(P.ExprAssignContext context)
        {

        }

        public override ICalcNode VisitExprFun(P.ExprFunContext context)
        {

        }

        public override ICalcNode VisitExprGrafic(P.ExprGraficContext context)
        {

        }

        public override ICalcNode VisitExprSignal(P.ExprSignalContext context)
        {

        }

        public override ICalcNode VisitExprObject(P.ExprObjectContext context)
        {

        }

        public override ICalcNode VisitExprMetSignal(P.ExprMetSignalContext context)
        {

        }

        public override ICalcNode VisitExprMet(P.ExprMetContext context)
        {

        }

        public override ICalcNode VisitExprOwner(P.ExprOwnerContext context)
        {

        }

        public override ICalcNode VisitExprParamProp(P.ExprParamPropContext context)
        {

        }

        public override ICalcNode VisitExprObjectProp(P.ExprObjectPropContext context)
        {

        }

        public override ICalcNode VisitExprSubParams(P.ExprSubParamsContext context)
        {

        }

        public override ICalcNode VisitExprPrev(P.ExprPrevContext context)
        {

        }

        public override ICalcNode VisitExprIf(P.ExprIfContext context)
        {

        }

        public override ICalcNode VisitExprWhile(P.ExprWhileContext context)
        {

        }

        //info
        public override ICalcNode VisitInfoSimple(P.InfoSimpleContext context)
        {
            return new CalcNodeInfo(context.IDENT().Symbol.Text);
        }

        public override ICalcNode VisitInfoArgs(P.InfoArgsContext context)
        {
            return new CalcNodeInfo(context.IDENT().Symbol.Text, context.INT().Symbol.Text.ToInt());
        }

        public override ICalcNode VisitInfoSignal(P.InfoSignalContext context)
        {
            var text = context.SIGNAL().Symbol.Text;
            return new CalcNodeInfo(text.Substring(1, text.Length - 2));
        }

        public override ICalcNode VisitInfoSignalArgs(P.InfoSignalArgsContext context)
        {
            var text = context.SIGNAL().Symbol.Text;
            return new CalcNodeInfo(text.Substring(1, text.Length - 2), context.INT().Symbol.Text.ToInt());
        }
    }
}