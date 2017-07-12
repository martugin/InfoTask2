using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий метод (получение сигнала)
    internal class MetSignalNode : TablikKeeperNode
    {
        public MetSignalNode(TablikKeeper keeper, ITerminalNode terminal, IExprNode parent) 
            : base(keeper, terminal)
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
            if (!(t is TablikObject) && !(t is ObjectType))
                AddError("Взятие сигнала от выражения, не являющего объектом");
            else
            {
                var type = t is ObjectType ? (ObjectType) t : ((TablikObject) t).ObjectType;
                var code = Token.Text.Substring(1, Token.Text.Length - 2);
                if (type.Signals.ContainsKey(code))
                {
                    Type = type.Signals[code];
                    var metSignals =  Keeper.GetMetSignals(Parent);
                    if (metSignals != null) metSignals.Add(code);
                }
                else AddError("Не найден сигнал");
            }
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Token.Text;
        }
    }
}