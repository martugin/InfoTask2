using BaseLibrary;
using CommonTypes;

namespace Generator
{
    internal class FunctionsGen : FunctionsBase, IContextable
    {
        //Контекст
        public string Context { get { return ""; } }
        protected override string ErrSourceCode { get { return "Генерация"; }}
        protected override IContextable Contextable { get { return this; } }

        //Условие для фильтрации списка функций
        protected override string FunsWhereCondition
        {
            get { return "(Functions.NotLoadCalc = False) AND (Functions.LoadGen = True)"; }
        }

        //Создание функции
        protected override CalcBaseFun CreateFun(string code, string ftype, int errNum)
        {
            return new ScalarGenFun(this, code, errNum);
        }
    }
}