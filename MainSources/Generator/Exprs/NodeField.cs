using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Значение поля таблицы
    internal class NodeField : NodeKeeper, INodeExpr
    {
        public NodeField(ParsingKeeper keeper, ITerminalNode terminal)
            : base(keeper, terminal)
        {
            if (terminal != null)
                _field = terminal.Symbol.Text;
        }

        protected override string NodeType { get { return "Field"; } }

        //Имя поля 
        private readonly string _field;
        
        //Вычисление значения
        public Mean Process(SubRows row)
        {
            return ((TablRow)row)[_field];
        }

        //Получение типа данных
        public DataType Check(TablStructItem row)
        {
            if (!row.Fields.ContainsKey(_field))
                AddError("Поле " + _field + " не найдено в исходной таблице");
            return row.Fields[_field];
        }
    }
}