using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    internal class PrevNode : TablikKeeperNode
    {
        public PrevNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args)
            : base(keeper, terminal, args)
        {
            if (args.Length == 2) Def = args[1];
            if (!(args[0].Type is TablikParam))
                AddError("Аргумент функции Абсолют должен задавать расчетный параметр", args[0].Token);
            else Param = (TablikParam)args[0].Type;
        }

        //Тип узла
        protected override string NodeType { get { return "Prev"; } }

        //Параметр
        public TablikParam Param { get; private set; }
        //Значение по умолчанию
        public IExprNode Def { get; private set; }

        //Определение типа
        public override void DefineType()
        {
            var dt = Def == null ? Param.DataType : Param.DataType.Add(Def.Type.DataType).AplySuperProcess(Param.SuperProcess);
            Type = new SimpleType(dt);    
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return "Absolute";
        }

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}