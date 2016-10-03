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
        public IMean Generate(SubRows row)
        {
            return ((TablRow)row)[_field];
        }

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            if (!tabl.Fields.ContainsKey(_field))
            {
                AddError("Поле не найдено в исходной таблице");
                return DataType.Error;
            }
            return tabl.Fields[_field];
        }
    }
}