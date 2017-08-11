using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение владельца
    internal class OwnerNode : TablikKeeperNode
    {
        public OwnerNode(TablikKeeper keeper, ITerminalNode terminal)
            : base(keeper, terminal) { }

        //Тип узла
        protected override string NodeType { get { return "Owner"; } }
        
        //Определение типа данных
        public override void DefineType()
        {
            if (Keeper.Param.Owner == null)
                AddError("Недопустимое получения владельца параметра");
            else Type = Keeper.Param.Owner;
        }

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}