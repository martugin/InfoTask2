using System.Linq;
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
            return new NodeText(context.element().Select(GoExpr));
        }

        public override Node VisitElementText(P.ElementTextContext context)
        {
            return _keeper.GetStringConst(context.TEXT());
        }

        public override Node VisitElementVoid(P.ElementVoidContext context)
        {
            return Go(context.voidProg());
        }

        public override Node VisitElementValue(P.ElementValueContext context)
        {
            return Go(context.valueProg());
        }

        public override Node VisitVoidProg(P.VoidProgContext context)
        {
            return new NodeVoidProg(context.voidExpr().Select(GoVoid).ToArray()); 
        }
        
        public override Node VisitValueProg(P.ValueProgContext context)
        {
            return new NodeValueProg(
                                new NodeVoidProg(context.voidExpr().Select(GoVoid).ToArray()), 
                                GoExpr(context.expr()));
        }

        //Выражения без значения

        public override Node VisitVoidExprVar(P.VoidExprVarContext context)
        {
            return new NodeVarSet(_keeper, context.IDENT(), GoExpr(context.expr()));
        }

        public override Node VisitVoidExprIf(P.VoidExprIfContext context)
        {
            return new NodeIfVoid(_keeper, context.IF(), 
                                               context.expr().Select(GoExpr).ToList(),
                                               context.voidProg().Select(GoVoid).ToList());
        }

        public override Node VisitVoidExprWhile(P.VoidExprWhileContext context)
        {
            return new NodeWhileVoid(_keeper, context.WHILE(),
                                                    GoExpr(context.expr()),
                                                    GoVoid(context.voidProg()));
        }

        public override Node VisitVoidExprOver(P.VoidExprOverContext context)
        {
            return new NodeOverVoid(_keeper, context.OVERTABL(), GoVoid(context.voidProg()));
        }

        public override Node VisitVoidExprSub(P.VoidExprSubContext context)
        {
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
            return Go(context.expr());
        }

        public override Node VisitExprIf(P.ExprIfContext context)
        {
            return new NodeIf(_keeper, context.IF(),
                                         context.expr().Select(GoExpr).ToList(),
                                         context.valueProg().Select(GoExpr).ToList());
        }

        public override Node VisitExprWhile(P.ExprWhileContext context)
        {
            return new NodeWhile(_keeper, context.WHILE(), 
                                              GoExpr(context.expr(0)), 
                                              GoExpr(context.valueProg()), 
                                              GoExpr(context.expr(1)));
        }

        public override Node VisitExprOver(P.ExprOverContext context)
        {
            return new NodeOver(_keeper, context.OVERTABL(), GoExpr(context.valueProg()));
        }

        public override Node VisitExprSub(P.ExprSubContext context)
        {
            return new NodeSub(_keeper, context.SUBTABL(),
                                           context.expr() == null ? null : GoExpr(context.expr()),
                                           GoExpr(context.valueProg(0)), 
                                           context.valueProg().Count() == 1 ? null : GoExpr(context.valueProg(1)));
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
            return new NodeFun(_keeper, context.IDENT(), GoList(context.pars()));
        }

        public override Node VisitExprUnary(P.ExprUnaryContext context)
        {
            return new NodeFun(_keeper, context.UNARY(), GoExpr(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new NodeFun(_keeper, fun, context.expr().Select(GoExpr).ToArray());
        }

        public override Node VisitExprFunParenLost(P.ExprFunParenLostContext context)
        {
            _keeper.AddError("Не закрытая скобка", context.LPAREN());
            return new NodeFun(_keeper, context.IDENT(), GoList(context.pars()));
        }

        public override Node VisitExprFunParenExtra(P.ExprFunParenExtraContext context)
        {
            _keeper.AddError("Лишняя закрывающаяся скобка", context.RPAREN(1));
            return new NodeFun(_keeper, context.IDENT(), GoList(context.pars()));
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
            return _keeper.GetStringConst(context.STRING());
        }

        public override Node VisitConsTime(P.ConsTimeContext context)
        {
            return _keeper.GetTimeConst(context.TIME());
        }
    }
}