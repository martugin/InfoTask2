using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Накопление данных в процессе разбора и генерации
    internal class GeneratorKeeper : ParsingKeeper
    {
        public GeneratorKeeper(string fieldName) 
            : base(fieldName) { }

        //Словарь внутренних переменных генерации
        private DicS<Var> _vars;
        public DicS<Var> Vars { get { return _vars ?? (_vars = new DicS<Var>()); } }
    }
}