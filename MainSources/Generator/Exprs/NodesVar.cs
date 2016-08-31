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

        //Получение значения
        public Mean Process(TablRow row)
        {
            return _var.Mean;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел присвоения переменной
     internal class NodeVarSet : Node, INodeVoid
     {
         public NodeVarSet(ITerminalNode terminal, Var v, INodeExpr nodeMean)  : base(terminal)
         {
             _var = v;
             _nodeMean = nodeMean;
         }

         protected override string NodeType { get { return "VarSet"; } }
         
         //Переменная
         private readonly Var _var;
         //Присваиваемое значение
         private readonly INodeExpr _nodeMean;

         //Обработка при генерации
         public void Process(TablRow row)
         {
             _var.Mean = _nodeMean.Process(row);
         }
     }
}