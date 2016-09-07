using CommonTypes;

namespace Generator
{
    //Скалярная функция для генерации
    internal class ScalarGenFunction : FunScalarBase
    {
        internal ScalarGenFunction(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Расчет значения
        internal IMean Calculate(IMean[] par, DataType resultType)
        {
            Functions.CurFun = this;
            if (ScalarRes.DataType != resultType)
                ScalarRes = new MomEdit(resultType);
            Functions.ScalarRes = ScalarRes;
            CalcScalar(par);
            return Functions.ScalarRes.CloneMean();
        }
    }
}