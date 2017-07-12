using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение свойства объкта
    internal class ObjectPropNode : TablikKeeperNode
    {
        public ObjectPropNode(TablikKeeper keeper, TablikObject ob, ITerminalNode terminal, ITerminalNode prop) 
            : base(keeper, terminal)
        {
            Object = ob;
            Prop = prop.Symbol;
        }

        //Тип узла
        protected override string NodeType { get { return "ObjectProp"; } }

        //Выражение, от которого берется метод
        public TablikObject Object { get; private set; }
        //Имя свойства
        public IToken Prop { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            if (!Object.UsedSignals.ContainsKey(Prop.Text))
                AddError("Не найдено свойство", Prop);
            else
            {
                var prop = Object.Props[Prop.Text];
                Type = new SimpleType(prop.DataType);
                Object.UsedProps.Add(Prop.Text, prop);
            }
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Prop.Text;
        }
    }
}