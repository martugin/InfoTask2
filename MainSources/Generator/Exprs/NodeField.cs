using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Значение поля таблицы
    internal class NodeField : Node, INodeExpr
    {
        public NodeField(ITerminalNode terminal)
            : base(terminal)
        {
            if (terminal != null)
                _field = terminal.Symbol.Text;
        }

        protected override string NodeType { get { return "Field"; } }

        //Имя поля 
        private readonly string _field;
        
        //Вычисление значения
        public Mean Process(TablRow row)
        {
            return row[_field];
        }
    }
}