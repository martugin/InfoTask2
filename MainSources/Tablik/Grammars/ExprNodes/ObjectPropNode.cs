using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение свойства объкта
    internal class ObjectPropNode : TablikKeeperNode
    {
        public ObjectPropNode(TablikKeeper keeper, IExprNode parent, ITerminalNode terminal, ITerminalNode prop) 
            : base(keeper, terminal)
        {
            Parent = parent;
            Prop = prop.Symbol;
        }

        //Тип узла
        protected override string NodeType { get { return "ObjectProp"; } }

        //Выражение, от которого берется свойство
        public IExprNode Parent { get; private set; }
        //Имя свойства
        public IToken Prop { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            if (Parent.Type is TablikObject)
            {
                var ob = (TablikObject) Parent.Type;
                if (ob.Props.ContainsKey(Prop.Text))
                {
                    var prop = ob.Props[Prop.Text];
                    Type = new SimpleType(prop.DataType);
                    if (!ob.UsedProps.ContainsKey(Prop.Text))
                        ob.UsedProps.Add(Prop.Text, prop);
                }
                else AddError("Не найдено свойство", Prop);
            }
            else if (Parent.Type is ObjectType || Parent.Type is BaseObjectType)
            {
                var metProps = Keeper.GetMetProps(Parent);
                if (((ITablikSignalType) Parent.Type).Connect.ObjectsColumns.ContainsKey(Prop.Text))
                    AddError("Не найдено свойство", Prop);
                else
                {
                    Type = new SimpleType(((ITablikSignalType)Parent.Type).Connect.ObjectsColumns[Prop.Text]);
                    if (metProps != null) metProps.Add(Prop.Text);
                }
            }
            else AddError("Недопустимое получение свойства", Prop);
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Prop.Text;
        }

        public override string ToTestString()
        {
            return ToTestWithChildren();
        }
    }
}