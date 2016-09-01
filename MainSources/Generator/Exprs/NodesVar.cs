using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Внутренняя переменная, содержащая промежуточный результат генерации
    internal class Var
    {
        public Var(string name)
        {
            Name = name;
        }

        //Имя переменной
        public string Name { get; private set; }
        //Тип данных
        public DataType DataType { get; set; }
        //Значение переменной
        public Mean Mean { get; set; }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел использования переменной
    internal class NodeVar : Node, INodeExpr
    {
        public NodeVar(ITerminalNode terminal, Var v) : base(terminal)
        {
            _var = v;
        }

        protected override string NodeType { get { return "Var"; } }

        //Переменная
        private readonly Var _var;

        //Получение типа данных
        public DataType Check(TablStructItem row)
        {
            return _var.DataType;
        }

        //Получение значения
        public Mean Process(SubRows row)
        {
            return _var.Mean;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел присвоения переменной
     internal class NodeVarSet : NodeKeeper, INodeVoid
     {
         public NodeVarSet(ParsingKeeper keeper, ITerminalNode terminal, Var v, INodeExpr nodeMean)  
             : base(keeper, terminal)
         {
             _var = v;
             _nodeMean = nodeMean;
         }

         protected override string NodeType { get { return "VarSet"; } }
         
         //Переменная
         private readonly Var _var;
         //Присваиваемое значение
         private readonly INodeExpr _nodeMean;

         //Проверка корректности выражений генерации
         public void Check(TablStructItem row)
         {
             _var.DataType.Add(_nodeMean.Check(row));
         }

         //Обработка при генерации
         public void Process(SubRows row)
         {
             _var.Mean = _nodeMean.Process(row);
         }
     }
}