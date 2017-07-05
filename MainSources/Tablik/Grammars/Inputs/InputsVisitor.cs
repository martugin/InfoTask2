using System.Linq;
using Antlr4.Runtime.Tree;
using CompileLibrary;
using P = Tablik.InputsParser;

namespace Tablik
{
    public class InputsVisitor : InputsBaseVisitor<Node>
    {
        public InputsVisitor(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly ParsingKeeper _keeper;

        //Обход дерева разбора
        public Node Go(IParseTree tree)
        {
            return tree == null ? null : Visit(tree);
        }
        public T Go<T>(IParseTree tree) where T : Node
        {
            return tree == null ? null : (T)Visit(tree);
        }

        //Обход разных типов узлов
        public override Node VisitProg(P.ProgContext context)
        {
            return new ListNode<InputNode>(context.param().Select(Go<InputNode>));
        }

        public override Node VisitParamArg(P.ParamArgContext context)
        {
            var arg = (InputArgNode)Go(context.arg());
            return new InputNode(arg.CodeToken, InputType.Simple, arg.TypeNode);
        }
        
        public override Node VisitParamConst(P.ParamConstContext context)
        {
            var arg = (InputArgNode)Go(context.arg());
            return new InputNode(arg.CodeToken, InputType.Simple, arg.TypeNode, null, (ConstNode)Go(context.constVal()));
        }

        public override Node VisitParamArray(P.ParamArrayContext context)
        {
            return new InputNode(context.IDENT(), InputType.Simple, (IdentNode)Go(context.DATATYPE()), (IdentNode)Go(context.ARRAY()));
        }

        public override Node VisitParamSignal(P.ParamSignalContext context)
        {
            return new InputNode(context.IDENT(), InputType.Signal, new IdentNode(context.SIGNAL()));
        }

        public override Node VisitParamClass(P.ParamClassContext context)
        {
            var list = (ListNode<IdentNode>)Go(context.identChain());
            return new InputNode(context.IDENT(), InputType.Param, list.Children[0], list.Children.Count == 2 ? list.Children[1] : null);
        }

        public override Node VisitArgDataType(P.ArgDataTypeContext context)
        {
            return new InputArgNode(context.IDENT(), context.DATATYPE());
        }

        public override Node VisitArgIdent(P.ArgIdentContext context)
        {
            return new InputArgNode(context.IDENT());
        }

        public override Node VisitIdentChain(P.IdentChainContext context)
        {
            return new ListNode<IdentNode>(context.IDENT().Select(Go<IdentNode>));
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