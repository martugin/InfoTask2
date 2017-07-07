using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий переменную
    internal class VarNode : TablikNode
    {
        public VarNode(ITerminalNode terminal, TablikVar v) 
            : base(terminal)
        {
            Var = v;
        }

        //Тип узла
        protected override string NodeType { get { return "Var"; } }

        //Переменная
        public TablikVar Var { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            Type = Var.Type;
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Var.Code;
        }
    }
}