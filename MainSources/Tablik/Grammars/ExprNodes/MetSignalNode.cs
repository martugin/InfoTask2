using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий метод (получение сигнала)
    internal class MetSignalNode : TablikKeeperNode
    {
        public MetSignalNode(TablikKeeper keeper, ITerminalNode terminal, IExprNode parent) 
            : base(keeper, terminal, parent)
        {
            Parent = parent;
        }

        //Тип узла
        protected override string NodeType { get { return "MetSignal"; } }

        //Выражение, от которого берется метод
        public IExprNode Parent { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            var t = Parent.Type;
            var code = Token.Text.Substring(1, Token.Text.Length - 2);
            var metSignals = Keeper.GetMetSignals(Parent);
            if (t is TablikObject || t is ObjectType)
            {
                var type = t is ObjectType ? (ObjectType)t : ((TablikObject)t).ObjectType;
                if (type.Signals.ContainsKey(code))
                {
                    Type = type.Signals[code];
                    if (metSignals != null) metSignals.Add(code);
                    else ((TablikObject) t).UsedSignals.Add((TablikSignal) Type);
                }
                else AddError("Не найден сигнал");
            } 
            else if (t is BaseObjectType)
            {
                var type = (BaseObjectType) t;
                if (type.Signals.ContainsKey(code))
                {
                    Type = type.Signals[code];
                    metSignals.Add(code);
                }
                else AddError("Не найден сигнал");
            }
            else AddError("Взятие сигнала от выражения, не являющегося объектом");
            if (Type == null) Type = new SimpleType();
        }

        public override string ToTestString()
        {
            return "MetSignal: " + Token.Text + " (" + Parent.ToTestString() + ")";
        }
    }
}