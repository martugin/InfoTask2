using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using CommonTypes;
using System.Linq;

namespace Generator
{
    //Узлы разбора правила генерации

    //Узел - вызывающий итерацию по строкам таблицы, базовый для Tabl, OverTabl
    internal interface INodeTabl
    {
        //Сгененирировать таблицу по исходным данным
        IEnumerable<SubRows> GetInitialRows(TablsList dataTabls);
    }
    
    //-----------------------------------------------------------------------------------------------------------------

    //Узел - проход по таблице
    internal class NodeRTabl : Node, INodeTabl
    {
        public NodeRTabl(ITerminalNode terminal, INodeExpr condition = null, NodeRSubTabl child = null) 
            : base(terminal)
        {
            if (terminal != null)
                _tablName = terminal.Symbol.Text;
            _condition = condition;
        }

        //Имя таблицы
        private readonly string _tablName;
        //Условие фильтрации или имя типа
        private readonly INodeExpr _condition;

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }

        public override string ToTestString()
        {
            return _condition == null ? ToTestWithChildren(_condition) : ToTestWithChildren();
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(TablsList dataTabls)
        {
            var tabl = dataTabls.Tabls[_tablName];
            if (_condition == null) return tabl.Rows[0].Values;
            if (_condition is NodeConst && tabl.SubTypes.ContainsKey(((NodeConst)_condition).Mean.String))
                return tabl.SubTypes[((NodeConst)_condition).Mean.String];
            return tabl.Rows[0].Values.Where(row =>_condition.Process(row).Boolean);
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - родительский ряд для таблицы
    internal class NodeROverTabl : Node, INodeTabl
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

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(TablsList dataTabls)
        {
            return new[] {dataTabls.Tabls[_tablName].Parent};
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Узел - перебор рядов подтаблицы
    internal class NodeRSubTabl : Node 
    {
        public NodeRSubTabl(ITerminalNode sub, INodeExpr condition = null)
            : base(sub)
        {
            Condition = condition;
        }
        
        //Условие фильтрации или имя типа
        protected INodeExpr Condition { get; private set; }
        //Выбор рядов из дочерней подтаблицы
        protected NodeRSubTabl Child { get; private set; } 

        //Тип узла
        protected override string NodeType { get { return "SubTabl"; } }

        public override string ToTestString()
        {
            return ToTestWithChildren(Condition, Child);
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(SubRows parent)
        {
            if (Condition == null) return parent.SubList;
            if (Condition is NodeConst && parent.SubTypes.ContainsKey(((NodeConst)Condition).Mean.String))
                return parent.SubTypes[((NodeConst)Condition).Mean.String];
            return parent.SubList.Where(row => Condition.Process(row).Boolean);
        }
    }
}