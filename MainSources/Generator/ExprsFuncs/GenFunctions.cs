using BaseLibrary;
using Calculation;

namespace Generator
{
    internal class GenFunctions : BaseFunctions, IContextable
    {
        //Контекст
        public string Context { get { return ""; } }
        protected override string ErrSourceCode { get { return "Генерация"; }}
        protected override IContextable Contextable { get { return this; } }

        //Условие для фильтрации списка функций
        protected override string FunsWhereCondition
        {
            get { return "(Functions.IsGen = True)"; }
        }

        //Создание функции
        protected override CalcBaseFun CreateFun(string code, string ftype, int errNum)
        {
            if (ftype == "Const")
                return new ConstGenFun(this, code, errNum);
            return new ScalarGenFun(this, code, errNum);
        }
    }
}