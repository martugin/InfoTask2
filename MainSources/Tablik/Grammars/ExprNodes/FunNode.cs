using System.Linq;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    internal class FunNode : TablikKeeperNode
    {
        public FunNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args)
            : base(keeper, terminal, args)
        {
            Fun = Keeper.Tablik.Funs[Token.Text];
        }

        //Имя узла
        protected override string NodeType { get { return "Fun"; }}

        //Функция
        public FunClass Fun { get; private set; }
        //Перегрузка
        public FunOverload Overload { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            var res = Fun.DefineOverload(Args.Select(x => x.Type).ToArray());
            Overload = res.Item1;
            Type = res.Item2;
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Overload.Code;
        }
    }
}