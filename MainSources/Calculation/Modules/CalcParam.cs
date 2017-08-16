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
            Keeper = new CalcKeeper(this);
        }

        //Модуль
        public CalcModule Module { get; private set; }

        //Накопитель ошибок
        internal CalcKeeper Keeper { get; private set; }
        //Корневой узел расчетного выражения
        internal OperatorNode RootNode { get; private set; }

        //Выходы
        private readonly SetS _inputs = new SetS();
        internal SetS Inputs { get { return _inputs; } }
        //Переменные
        private readonly SetS _vars = new SetS();
        internal SetS Vars { get { return _vars; } }

        //Владелец
        public CalcParam Owner { get; private set; }
        //Подпараметры
        private readonly DicS<CalcParam> _subParams = new DicS<CalcParam>();
        public DicS<CalcParam> SubParams { get { return _subParams; } }
        
        //Разбор скомпилированного выражения
        public void ParseExpr()
        {
            RootNode = (OperatorNode)new CalcExprParsing(Keeper, "Expr", CompiledExpr).ResultTree;
        }

        //Вычисление значения
        public void Calculate()
        {
            
        }
    }
}