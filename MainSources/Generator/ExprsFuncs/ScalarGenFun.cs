using CommonTypes;

namespace Generator
{
    //Интерфейс функций для генерации
    internal interface IGenFun
    {
        IReadMean Calculate(IReadMean[] par, DataType resultType);
    }

    //---------------------------------------------------------------------------------------------------

    //Константа
    internal class ConstGenFun : ConstBaseFun, IGenFun
    {
        public ConstGenFun(BaseFunctions funs, string code, int errNum)
            : base(funs, code, errNum) { }

        //Расчет значения
        public IReadMean Calculate(IReadMean[] par, DataType resultType)
        {
            return (IReadMean)Fun();
        }
    }

    //---------------------------------------------------------------------------------------------------

    //Скалярная функция для генерации
    internal class ScalarGenFun : ScalarBaseFun, IGenFun
    {
        internal ScalarGenFun(BaseFunctions funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Расчет значения
        public IReadMean Calculate(IReadMean[] par, DataType resultType)
        {
            Functions.CurFun = this;
            Functions.SetScalarDataType(resultType);
            Functions.CalcScalarFun(par, () => Fun(par));
            return Functions.ScalarRes.ToMom();
        }
    }
}