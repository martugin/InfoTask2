using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Ключевое слово Пустой
    internal class VoidNode : TablikNode
    {
        public VoidNode(ITerminalNode terminal) 
            : base(terminal) { }

        //Тип узла
        protected override string NodeType { get { return "Void"; } }

        //Определение типа данных
        public override void DefineType()
        {
            Type = new SimpleType(DataType.Void);
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Token.Text;
        }

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}