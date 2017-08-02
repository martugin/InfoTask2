using BaseLibrary;
using CompileLibrary;

namespace Calculation
{
    //Один расчетный параметр 
    internal class CalcParam : BaseCalcParam
    {
        public CalcParam(IRecordRead rec, bool isSubParam)
            : base(rec, isSubParam)
        {
            CompiledExpr = rec.GetString("CompiledExpr");
        }

        //Корневой узел расчетного выражения
        public OperatorNode RootNode { get; private set; }
        
        //Разбор скомпилированного выражения
        public void ParseExpr()
        {
            
        }

        //Вычисление значения
        public void Calculate()
        {
            
        }
    }
}