using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Узел, возвращаемый, когда не удалось определить тип узла
    internal class ErrorNode : TablikNode
    {
        public ErrorNode(ITerminalNode terminal) 
            : base(terminal) { }

        protected override string NodeType { get { return "Error"; }}

        public override void DefineType()
        {
            Type = new SimpleType(DataType.Error);
        }

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}