using CommonTypes;

namespace Generator
{
    //Интерфейс функций для генерации
    internal interface IGenFun
    {
        IMean Calculate(IMean[] par, DataType resultType);
    }

    //---------------------------------------------------------------------------------------------------

    //Константа
    internal class ConstGenFun : ConstBaseFun, IGenFun
    {
        public ConstGenFun(FunctionsBase funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Расчет значения
        public IMean Calculate(IMean[] par, DataType resultType)
        {
            return (IMean)Fun();
        }
    }

    //---------------------------------------------------------------------------------------------------

    //Скалярная функция для генерации
    internal class ScalarGenFun : ScalarBaseFun, IGenFun
    {
        internal ScalarGenFun(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Расчет значения
        public IMean Calculate(IMean[] par, DataType resultType)
        {
            Functions.CurFun = this;
            Functions.SetScalarDataType(resultType);
            Functions.CalcScalarFun(par, () => Fun(par));
            return Functions.ScalarRes.ToMean();
        }
    }
}