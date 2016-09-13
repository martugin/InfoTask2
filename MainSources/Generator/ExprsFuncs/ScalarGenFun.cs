using CommonTypes;

namespace Generator
{
    //Скалярная функция для генерации
    internal class ScalarGenFun : ScalarBaseFun
    {
        internal ScalarGenFun(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Расчет значения
        internal IMean Calculate(IMean[] par, DataType resultType)
        {
            Functions.CurFun = this;
            Functions.SetScalarDataType(resultType);
            Functions.CalcScalarFun(par, () => Fun(par));
            return Functions.ScalarRes.CloneMean();
        }
    }
}