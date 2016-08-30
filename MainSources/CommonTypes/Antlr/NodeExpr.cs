using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Базовый класс для узлов, у которых может быть вычислено значение
    public abstract class NodeExpr : Node, INodeText
    {
        //Вычисляемое значение
        protected NodeExpr(ITerminalNode terminal)
            : base(terminal) { }
        
        //Возвращаемое значение
        public abstract Mean Process();
        //Значение как текст
        public string GetText()
        {
            return Process().String;
        }
    }

    //------------------------------------------------------------------------------------------------------
    //Базовый класс для узлов без значения
    public abstract class NodeVoid : Node
    {
        //Вычисляемое значение
        protected NodeVoid(ITerminalNode terminal)
            : base(terminal) { }

        //Возвращаемое значение
        public abstract void Process();
    }
}