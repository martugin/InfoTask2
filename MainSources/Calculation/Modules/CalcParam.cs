using BaseLibrary;
using CompileLibrary;

namespace Calculation
{
    //Один расчетный параметр 
    public class CalcParam : BaseCalcParam
    {
        public CalcParam(CalcModule module, IRecordRead rec, bool isSubParam)
            : base(rec, isSubParam)
        {
            Module = module;
            CompiledExpr = rec.GetString("CompiledExpr");
            Keeper = new ParsingKeeper();
        }

        //Модуль
        public CalcModule Module { get; private set; }

        //Накопитель ошибок
        internal ParsingKeeper Keeper { get; private set; }
        //Корневой узел расчетного выражения
        public OperatorNode RootNode { get; private set; }

        //Словарь переменных
        private readonly DicS<CalcVar> _vars = new DicS<CalcVar>();
        internal DicS<CalcVar> Vars { get { return _vars; } }

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