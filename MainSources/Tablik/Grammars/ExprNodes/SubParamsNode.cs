using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Узел функции Подпараметры
    internal class SubParamsNode : TablikKeeperNode
    {
        public SubParamsNode(TablikKeeper keeper, ITerminalNode terminal, IExprNode expr) 
            : base(keeper, terminal, expr) { }

        //Тип узла
        protected override string NodeType { get { return "SubParams"; } }

        //Параметр
        public TablikParam Param { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            Type = new SimpleType(DataType.Void);
            if (!(Args[0].Type is TablikParam) || ((TablikParam) Args[0].Type).Params.Count == 0)
                AddError("Аргументом функция Подпараметры может быть только параметр с подпараметрами");
            else
            {
                Param = (TablikParam) Args[0].Type;
                Keeper.Param.BaseParams.Add(Param);
            }
        }
    }
}