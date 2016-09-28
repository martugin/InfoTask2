using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел использования переменной
    internal class NodeVar : NodeKeeper, INodeExpr
    {
        public NodeVar(GenKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            var name = terminal.Symbol.Text;
            if (!keeper.Vars.ContainsKey(name))
                AddError("Использование неприсвоеной переменной");
            else _var = keeper.Vars[name];
        }

        protected override string NodeType { get { return "Var"; } }

        //Переменная
        private readonly Var _var;

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            if (_var == null) return DataType.Error;
            return _var.DataType;
        }

        //Получение значения
        public IMean Generate(SubRows row)
        {
            return _var.Mean;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел присвоения переменной
     internal class NodeVarSet : NodeKeeper, INodeVoid
     {
         public NodeVarSet(GenKeeper keeper, ITerminalNode terminal, INodeExpr nodeMean)  
             : base(keeper, terminal)
         {
             var name = terminal.Symbol.Text;
             _var = keeper.Vars.Add(name, new Var(name));
             _nodeMean = nodeMean;
         }

         protected override string NodeType { get { return "VarSet"; } }
         public override string ToTestString()
         {
             return ToTestWithChildren(_nodeMean);
         }

         //Переменная
         private readonly Var _var;
         //Присваиваемое значение
         private readonly INodeExpr _nodeMean;

         //Проверка корректности выражений генерации
         public void Check(TablStruct tabl)
         {
             _var.DataType = _var.DataType.Add(_nodeMean.Check(tabl));
         }

         //Обработка при генерации
         public void Generate(SubRows row)
         {
             _var.Mean = _nodeMean.Generate(row);
         }
     }
}