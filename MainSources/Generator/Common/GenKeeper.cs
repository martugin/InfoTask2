using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Внутренняя переменная, содержащая промежуточный результат генерации
    internal class Var
    {
        public Var(string name)
        {
            Name = name;
        }

        //Имя переменной
        public string Name { get; private set; }
        //Тип данных
        public DataType DataType { get; set; }
        //Значение переменной
        public Mean Mean { get; set; }
    }

    //----------------------------------------------------------------------------------------------------
    //Накопитель ошибок и переменных генерации
    internal class GenKeeper : ParsingKeeper
    {
        public override void SetFieldName(string fieldName)
        {
            base.SetFieldName(fieldName);
            Vars.Clear();
        }

        //Словарь внутренних переменных генерации
        private readonly DicS<Var> _vars = new DicS<Var>(); 
        public DicS<Var> Vars { get { return _vars; }}
    }
}