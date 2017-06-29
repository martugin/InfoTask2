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
        internal ISyntacticNode Go(IParseTree tree)
        {
            if (tree == null) return null;
            return (ISyntacticNode)Visit(tree);
        }


        //Выражения без значения



        //Выражения со значением



        //Список аргументов функции
        public override Node VisitParamsList(P.ParamsListContext context)
        {
            return new ListNode<ISyntacticNode>(context.expr().Select(Go));
        }
        public override Node VisitParamsEmpty(P.ParamsEmptyContext context)
        {
            return new ListNode<ISyntacticNode>(new List<ISyntacticNode>());
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