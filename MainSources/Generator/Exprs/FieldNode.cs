using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Значение поля таблицы
    internal class FieldNode : KeeperNode, IExprNode
    {
        public FieldNode(ParsingKeeper keeper, ITerminalNode terminal)
            : base(keeper, terminal)
        {
            if (terminal != null)
                _field = terminal.Symbol.Text;
        }

        protected override string NodeType { get { return "Field"; } }

        //Имя поля 
        private readonly string _field;
        
        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            if (tabl == null)
            {
                AddError("Попытка получения значения поля при не заданной таблице");
                return DataType.Error;
            }
            if (!tabl.Fields.ContainsKey(_field))
            {
                AddError("Поле не найдено в исходной таблице");
                return DataType.Error;
            }
            return tabl.Fields[_field];
        }

        //Вычисление значения
        public IMean Generate(SubRows row)
        {
            return row[_field];
        }
    }
}