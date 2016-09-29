using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using CommonTypes;

namespace Generator
{
    //Фильтрация списка рядов подтаблицы
    internal class RowsSelector
    {
        public RowsSelector(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        private readonly ParsingKeeper _keeper;

        //Проверка выражения
        public void Check(INodeExpr condition, IToken tablToken, TablStruct tabl)
        {
            if (condition != null)
                if (condition.Check(tabl) != DataType.Boolean && (!(condition is NodeConst) || condition.Check(tabl) != DataType.String))
                    _keeper.AddError("Недопустимый тип данных условия", tablToken);
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> SelectRows(INodeExpr condition, SubRows parent)
        {
            if (condition == null) return parent.SubList;
            if (condition is NodeConst && parent.SubTypes.ContainsKey(((NodeConst)condition).Mean.String))
                return parent.SubTypes[((NodeConst)condition).Mean.String];
            return parent.SubList.Where(row => condition.Generate(row).Boolean);
        }
    }
}