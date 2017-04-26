using System;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;
using CompileLibrary;

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
        public IReadMean Mean { get; set; }
    }

    //----------------------------------------------------------------------------------------------------
    //Накопитель ошибок и переменных генерации
    internal class GenKeeper : ParsingKeeper
    {
        public GenKeeper(ModuleGenerator generator)
        {
            Generator = generator;
        }

        public override void SetFieldName(string fieldName)
        {
            base.SetFieldName(fieldName);
            Vars.Clear();
        }

        //Ссылка на генератор
        internal ModuleGenerator Generator { get; private set; }

        //Словарь внутренних переменных генерации
        private readonly DicS<Var> _vars = new DicS<Var>(); 
        public DicS<Var> Vars { get { return _vars; }}

        //Методы создания узлов - констант разного типа
        protected override Node MakeConstNode(ITerminalNode terminal, bool b)
        {
            return new GenConstNode(terminal, b);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, int i)
        {
            return new GenConstNode(terminal, i);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, double r)
        {
            return new GenConstNode(terminal, r);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, DateTime d)
        {
            return new GenConstNode(terminal, d);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, string s)
        {
            return new GenConstNode(terminal, s);
        }
    }
}