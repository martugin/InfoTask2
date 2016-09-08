using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    internal partial class FunctionsCalc : FunctionsBase
    {
        //Условие для фильтрации списка функций
        protected override string FunsWhereCondition
        {
            get { return " (Functions.NotLoadCalc = False)"; }
        }

        //Создание функций разных типов
        protected override FunCalcBase CreateFun(string code, string ftype, int errNum)
        {
            switch (ftype)
            {
                case "Scalar":
                    return new ScalarFunction(this, code, errNum);
                case "Const":
                    return new ConstFunction(this, code, errNum);
            }
            return null;
        }

        //Код модуля
        protected override string ErrSourceCode
        {
            get { throw new System.NotImplementedException(); }
        }

        //Текущий порожденный расчетный параметр
        protected override IContextable Contextable
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}