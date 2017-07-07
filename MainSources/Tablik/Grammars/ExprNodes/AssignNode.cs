using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Присвоение
    internal class AssignNode : TablikKeeperNode
    {
        public AssignNode(TablikKeeper keeper, ITerminalNode assign, ITerminalNode v, IExprNode expr, IExprNode type = null) 
            : base(keeper, assign, expr)
        {
            var vcode = v.Symbol.Text;
            var vars = Keeper.Param.Vars;
            if (vars.ContainsKey(vcode)) Var = vars[vcode];
            else vars.Add(vcode, Var = new TablikVar(vcode, type == null ? null : type.Type));
        }

        //Тип узла
        protected override string NodeType { get { return "Assign"; }}

        //Переменная
        public TablikVar Var { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            Var.Type = Var.Type.Add(Args[0].Type);
            if (Var.Type.DataType == DataType.Error)
                AddError("Недопустипое присвоение переменной");
            Type = new SimpleType(DataType.Void);
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Var.Code;
        }
    }
}