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

        public ListNode GoList(IParseTree tree)
        {
            return (ListNode)Go(tree);
        }

        //Обход разных типов узлов

        public override Node VisitParamArg(P.ParamArgContext context)
        {
            
        }

        public override Node VisitArgDataType(P.ArgDataTypeContext context)
        {
            return new InputsArgNode(context.IDENT(), context.DATATYPE());
        }

        public override Node VisitArgIdent(P.ArgIdentContext context)
        {
            return new InputsArgNode(context.IDENT());
        }

        public override Node VisitIdentChain(P.IdentChainContext context)
        {
            return new ListNode(context.IDENT().Select(Go));
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