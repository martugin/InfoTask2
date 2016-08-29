using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Базовый класс для узлов, у которых может быть вычислено значение
    public abstract class NodeExpr : Node, INodeText
    {
        //Вычисляемое значение
        protected NodeExpr(ITerminalNode terminal)
            : base(terminal) { }

        protected NodeExpr(IToken token)
            : base(token) { }

        //Возвращаемое значение
        public abstract Mean GetMean();
        //Значение как текст
        public string GetText()
        {
            return GetMean().String;
        }
    }
}