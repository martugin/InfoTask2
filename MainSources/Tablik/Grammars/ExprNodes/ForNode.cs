using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    internal class ForNode : TablikKeeperNode
    {
        public ForNode(TablikKeeper keeper, ITerminalNode terminal, TablikVar v, IExprNode array, IExprNode prog) 
            : base(keeper, terminal, array, prog)
        {
            Var = v;
        }

        //Тип узла
        protected override string NodeType { get { return "For"; } }

        //Перемеменная цикла
        public TablikVar Var { get; private set; }

        //Проверка типов данных
        public override void DefineType()
        {
            Type = new SimpleType(DataType.Void);
        }
    }
}