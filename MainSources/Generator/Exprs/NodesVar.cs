using Antlr4.Runtime.Tree;
using BaseLibrary;
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
    //Накопление данных в процессе разбора, включая переменные
    internal class GeneratorKeeper : ParsingKeeper
    {
        public GeneratorKeeper(string fieldName) 
            : base(fieldName) { }

        //Словарь внутренних переменных генерации
        private DicS<Var> _vars;
        public DicS<Var> Vars { get { return _vars ?? (_vars = new DicS<Var>()); } }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел использования переменной
    internal class NodeVar : NodeExpr
    {
        public NodeVar(ITerminalNode terminal, Var v) : base(terminal)
        {
            _var = v;
        }

        protected override string NodeType { get { return "Var"; } }

        //Переменная
        private readonly Var _var;

        //Получение значения
        public override Mean Process()
        {
            return _var.Mean;
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел присвоения переменной
     internal class NodeVarSet : NodeVoid
     {
         public NodeVarSet(ITerminalNode terminal, Var v, NodeExpr nodeMean)  : base(terminal)
         {
             _var = v;
             _nodeMean = nodeMean;
         }

         protected override string NodeType { get { return "VarSet"; } }
         
         //Переменная
         private readonly Var _var;
         //Присваиваемое значение
         private readonly NodeExpr _nodeMean;

         //Обработка при генерации
         public override void Process()
         {
             _var.Mean = _nodeMean.Process();
         }
     }

    
}