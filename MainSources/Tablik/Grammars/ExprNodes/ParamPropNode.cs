using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение свойства параметра
    internal class ParamPropNode : TablikKeeperNode
    {
        public ParamPropNode(TablikKeeper keeper, ITerminalNode terminal, ITerminalNode prop) 
            : base(keeper, terminal)
        {
            IsMet = true;
            Prop = prop.Symbol;
        }

        //Тип узла
        protected override string NodeType { get { return "ParamProp"; } }

        //Имя свойства
        public IToken Prop { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            Type = new SimpleType(Keeper.FunsChecker.ParamProps[Prop.Text]);
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Prop.Text;
        }
    }
}