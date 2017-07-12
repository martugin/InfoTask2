using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Узел цикла While
    internal class WhileNode : TablikKeeperNode
    {
        public WhileNode(TablikKeeper keeper, ITerminalNode terminal, IExprNode condition, IExprNode prog) 
            : base(keeper, terminal, condition, prog) { }

        //Тип узла
        protected override string NodeType { get { return "While"; } }

        //Проверка типов данных
        public override void DefineType()
        {
            if (Args[0].Type.DataType != DataType.Boolean)
                AddError("Недопустимый тип данных условия");
            Type = new SimpleType(DataType.Void);
        }
    }
}