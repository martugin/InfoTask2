using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Generator
{
    //Вычисление значения функции или операции
    internal class FunNode : KeeperNode, IExprNode
    {
        public FunNode(GenKeeper keeper, ITerminalNode terminal, //Имя функции
                                params IExprNode[] args) //Аргументы
            : base(keeper, terminal)
        {
            _args = args;
        }

        public FunNode(GenKeeper keeper, ITerminalNode terminal, //Имя функции
                                ListNode argsList) //Узел с аргументами
            : base(keeper, terminal)
        {
            _args = argsList.Nodes.Cast<IExprNode>().ToArray();
        }

        protected override string NodeType
        {
            get { return "Fun"; }
        }

        //Аргументы
        private readonly IExprNode[] _args;
        //Функция для расчета
        private IGenFun _fun;
        //Возвращаемый тип данных
        private DataType _resultType = DataType.Error;

        public override string ToTestString()
        {
            return ToTestWithChildren(_args);
        }

        //Получение типа данных
        public DataType Check(ITablStruct tabl)
        {
            var generator = Keeper.Generator;
            var funs = generator.FunsChecker.Funs;
            if (!funs.ContainsKey(Token.Text))
                AddError("Неизвестная функция");
            else
            {
                var fs = funs[Token.Text].DefineOverload( _args.Select(a => a.Check(tabl)).ToArray());
                if (fs == null)
                {
                    AddError("Недопустимые типы данных параметров функции");
                    return DataType.Error;
                }
                _fun = (IGenFun) generator.Functions.Funs[fs.Overload.Code];
                return _resultType = fs.DataType;
            }
            return DataType.Error;
        }

        //Вычисленное значение
        public IReadMean Generate(SubRows row)
        {
            return _fun.Calculate(_args.Select(a => a.Generate(row)).ToArray(), _resultType);
        }
    }
}