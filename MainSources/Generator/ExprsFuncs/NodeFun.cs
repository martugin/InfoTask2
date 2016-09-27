using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Вычисление значения функции или операции
    internal class NodeFun : NodeKeeper, INodeExpr
    {
        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                       params INodeExpr[] args) //Аргументы
            : base(keeper, terminal)
        {
            _args = args;
        }

        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                       NodeList argsList) //Узел с аргументами
            : base(keeper, terminal)
        {
            _args = argsList.Children.Cast<INodeExpr>().ToArray();
        }

        protected override string NodeType
        {
            get { return "Fun"; }
        }

        //Аргументы
        private readonly INodeExpr[] _args;
        //Функция для расчета
        private IGenFun _fun;
        //Тип данных результата
        private DataType _resultType;

        public override string ToTestString()
        {
            return ToTestWithChildren(_args);
        }

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            var generator = ((GenKeeper) Keeper).Generator;
            var t = generator.FunsChecker.DefineFun(Token.Text, _args.Select(a => a.Check(tabl)).ToArray());
            if (t.Item2 == DataType.Error)
                AddError(t.Item1);
            else _fun = (IGenFun)generator.Functions.Funs[t.Item1];
            return _resultType = t.Item2;
        }

        //Вычисленное значение
        public IMean Generate(SubRows row)
        {
            return _fun.Calculate(_args.Select(a => a.Generate(row)).ToArray(), _resultType);
        }
    }
}