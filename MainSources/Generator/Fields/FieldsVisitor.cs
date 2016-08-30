using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using Generator.Fields;
using P = Generator.Fields.FieldsParsemes;

namespace Generator
{
    internal class FieldsVisitor : FieldsParsemesBaseVisitor<Node> 
    {
        public FieldsVisitor(GeneratorKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly GeneratorKeeper _keeper;

        //Обход дерева разбора, возвращаются разные типы вершин
        public Node Go(IParseTree tree)
        {
            if (tree == null) return null;
            return Visit(tree);
        }
        public NodeExpr GoExpr(IParseTree tree)
        {
            return (NodeExpr)Go(tree);
        }
        public NodeVoid GoVoid(IParseTree tree)
        {
            return (NodeVoid)Go(tree);
        }
        public NodeList GoList(IParseTree tree)
        {
            return (NodeList)Go(tree);
        }

        public override Node VisitFieldGen(P.FieldGenContext context)
        {
            return new NodeList(context.element().Select(Go));
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
            return new NodeList(context.voidExpr().Select(Go));
        }
        
        public override Node VisitValueProg(P.ValueProgContext context)
        {
            var list = context.voidExpr().Select(Go).ToList();
            list.Add(Go(context.expr()));
            return new NodeList(list);
        }

        //Выражения без значения

        public override Node VisitVoidExprVar(P.VoidExprVarContext context)
        {
            var name = context.IDENT().Symbol.Text;
            var v = _keeper.Vars.Add(name, new Var(name));
            return new NodeVarSet(context.IDENT(), v, GoExpr(context.expr()));
        }

        public override Node VisitVoidExprIf(P.VoidExprIfContext context)
        {
            return new NodeIfVoid(context.IF(), 
                                               context.expr().Select(GoExpr).ToList(),
                                               context.voidProg().Select(GoVoid).ToList());
        }

        public override Node VisitVoidExprWhile(P.VoidExprWhileContext context)
        {
            return new NodeWhileVoid(context.WHILE(),
                                                    GoExpr(context.expr()),
                                                    GoVoid(context.voidProg()));
        }

        public override Node VisitVoidExprOver(P.VoidExprOverContext context)
        {
            return new NodeOverVoid(context.OVERTABL(), GoVoid(context.voidProg()), _keeper);
        }

        public override Node VisitVoidExprSub(P.VoidExprSubContext context)
        {
            return new NodeSubVoid(context.SUBTABL(),  
                                                  context.expr() == null ? null : GoExpr(context.expr()),
                                                  GoVoid(context.voidProg()), _keeper);
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
            return new NodeIf(context.IF(),
                                         context.expr().Select(GoExpr).ToList(),
                                         context.valueProg().Select(GoExpr).ToList());

        }

        public override Node VisitExprWhile(P.ExprWhileContext context)
        {
            return new NodeWhile(context.WHILE(), 
                                              GoExpr(context.expr(0)), 
                                              GoExpr(context.valueProg()), 
                                              GoExpr(context.expr(1)));
        }

        public override Node VisitExprOver(P.ExprOverContext context)
        {
            return new NodeOver(context.OVERTABL(), GoExpr(context.valueProg()), _keeper);
        }

        public override Node VisitExprSub(P.ExprSubContext context)
        {
            return new NodeSub(context.SUBTABL(),
                                           context.expr() == null ? null : GoExpr(context.expr()),
                                           GoExpr(context.valueProg(0)), 
                                           context.valueProg().Count() == 1 ? null : GoExpr(context.valueProg(1)),
                                           _keeper);
        }

        public override Node VisitExprIdent(P.ExprIdentContext context)
        {
            return new NodeField(context.IDENT());
        }

        public override Node VisitExprFun(P.ExprFunContext context)
        {
            return new NodeFun(context.IDENT(), GoList(context.pars()));
        }

        public override Node VisitExprUnary(P.ExprUnaryContext context)
        {
            return new NodeFun(context.UNARY(), GoExpr(context.expr()));
        }

        public override Node VisitExprOper(P.ExprOperContext context)
        {
            var fun = (ITerminalNode)context.children[1];
            return new NodeFun(fun, context.expr().Select(GoExpr).ToArray());
        }

        public override Node VisitExprFunParenLost(P.ExprFunParenLostContext context)
        {
            _keeper.AddError("Не закрытая скобка", context.LPAREN());
            return new NodeFun(context.IDENT(), GoList(context.pars()));
        }

        public override Node VisitExprFunParenExtra(P.ExprFunParenExtraContext context)
        {
            _keeper.AddError("Лишняя закрывающаяся скобка", context.RPAREN(1));
            return new NodeFun(context.IDENT(), GoList(context.pars()));
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
    }
}