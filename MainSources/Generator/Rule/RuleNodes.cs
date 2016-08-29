using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узлы разбора правила генерации

    //Узел - вызывающий итерацию по строкам таблицы, базовый для Tabl, OverTabl, SubTabl
    internal abstract class NodeIter : Node
    {
        protected NodeIter(ITerminalNode terminal) 
            : base(terminal) { }
    }

    //-----------------------------------------------------------------------------------------------------------------

    //Узел - проход по таблице
    internal class NodeRTabl : NodeIter
    {
        public NodeRTabl(ITerminalNode terminal, NodeExpr condition = null) 
            : base(terminal)
        {
            if (terminal != null)
                _tablName = terminal.Symbol.Text;
            _condition = condition;
        }

        //Имя таблицы
        private string _tablName;
        //Условие фильтрации или имя типа
        private readonly NodeExpr _condition;

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }

        public override string ToTestString()
        {
            return _condition == null ? ToTestWithChildren(_condition) : ToTestWithChildren();
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - родительский ряд для таблицы
    internal class NodeROverTabl : NodeIter
    {
        public NodeROverTabl(ITerminalNode tabl, ITerminalNode over)
            : base(over)
        {
            if (tabl != null)
                _tablName = tabl.Symbol.Text;
        }

        //Имя таблицы
        private string _tablName;

        //Тип узла
        protected override string NodeType { get { return "OverTabl"; } }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - перебор рядов подтаблицы
    internal class NodeRSubTabl : NodeIter
    {
        public NodeRSubTabl(ITerminalNode sub, NodeExpr condition = null)
            : base(sub)
        {
            _condition = condition;
        }

        //Родительский узел
        internal NodeIter Parent { get; set; }
        //Условие фильтрации или имя типа
        private readonly NodeExpr _condition;

        //Тип узла
        protected override string NodeType { get { return "SubTabl"; } }

        public override string ToTestString()
        {
            return _condition == null ? ToTestWithChildren(Parent, _condition) : ToTestWithChildren(Parent);
        }
    }
}