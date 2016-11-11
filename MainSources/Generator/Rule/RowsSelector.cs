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
                if (condition.Check(tabl) != DataType.Boolean)
                    _keeper.AddError("Недопустимый тип данных условия", tablToken);
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> SelectRows(INodeExpr condition, SubRows parent)
        {
            if (condition == null) return parent.SubList;
            return parent.SubList.Where(row => condition.Generate(row).Boolean);    
        }
    }
}