using System.Collections.Generic;
using System.Linq;
using CommonTypes;

namespace Generator
{
    //Фильтрация списка рядов подтаблицы
    internal class RowsConditionChecker
    {
        public RowsConditionChecker(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        private readonly ParsingKeeper _keeper;

        //Проверка выражения
        public void Check(INodeExpr condition, TablStructItem row)
        {
            if (!(condition is NodeConst) && condition.Check(row) != DataType.Boolean)
                _keeper.AddError("Недопустимый тип данных условия", condition.Token);
        }

        //Выбрать ряды для геренации
        public IEnumerable<SubRows> GetInitialRows(INodeExpr condition, SubRows parent)
        {
            if (condition == null) return parent.SubList;
            if (condition is NodeConst && parent.SubTypes.ContainsKey(((NodeConst)condition).Mean.String))
                return parent.SubTypes[((NodeConst)condition).Mean.String];
            return parent.SubList.Where(row => condition.Process(row).Boolean);
        }
    }
}