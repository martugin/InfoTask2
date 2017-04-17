using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел использования переменной
    internal class VarNode : KeeperNode, IExprNode
    {
        public VarNode(GenKeeper keeper, ITerminalNode terminal) 
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
        public DataType Check(ITablStruct tabl)
        {
            if (_var == null) return DataType.Error;
            return _var.DataType;
        }

        //Получение значения
        public IReadMean Generate(SubRows row)
        {
            return _var.Mean;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел присвоения переменной
     internal class VarSetNode : KeeperNode, IVoidNode
     {
         public VarSetNode(GenKeeper keeper, ITerminalNode terminal, IExprNode nodeMean)  
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
         private readonly IExprNode _nodeMean;

         //Проверка корректности выражений генерации
         public void Check(ITablStruct tabl)
         {
             if (tabl.Fields.ContainsKey(_var.Name))
                AddError("Имя переменной не должно совпадать с именем поля таблицы");
             _var.DataType = _var.DataType.Add(_nodeMean.Check(tabl));
         }

         //Обработка при генерации
         public void Generate(SubRows row)
         {
             _var.Mean = _nodeMean.Generate(row);
         }
     }
}