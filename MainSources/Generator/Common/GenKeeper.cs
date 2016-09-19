using System;
using Antlr4.Runtime.Tree;
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
        public IMean Mean { get; set; }
    }

    //----------------------------------------------------------------------------------------------------
    //Накопитель ошибок и переменных генерации
    internal class GenKeeper : ParsingKeeper
    {
        public GenKeeper(TablGenerator generator)
        {
            Generator = generator;
        }

        public override void SetFieldName(string fieldName)
        {
            base.SetFieldName(fieldName);
            Vars.Clear();
        }

        //Ссылка на генератор
        internal TablGenerator Generator { get; private set; }

        //Словарь внутренних переменных генерации
        private readonly DicS<Var> _vars = new DicS<Var>(); 
        public DicS<Var> Vars { get { return _vars; }}

        //Методы создания узлов - констант разного типа
        protected override Node MakeNodeConst(ITerminalNode terminal, bool b)
        {
            return new NodeGenConst(terminal, b);
        }
        protected override Node MakeNodeConst(ITerminalNode terminal, int i)
        {
            return new NodeGenConst(terminal, i);
        }
        protected override Node MakeNodeConst(ITerminalNode terminal, double r)
        {
            return new NodeGenConst(terminal, r);
        }
        protected override Node MakeNodeConst(ITerminalNode terminal, DateTime d)
        {
            return new NodeGenConst(terminal, d);
        }
        protected override Node MakeNodeConst(ITerminalNode terminal, string s)
        {
            return new NodeGenConst(terminal, s);
        }
    }
}