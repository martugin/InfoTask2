using System.Linq;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    internal class FunNode : TablikKeeperNode
    {
        public FunNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args)
            : base(keeper, terminal, args)
        {
            Fun = Keeper.FunsChecker.Funs[Token.Text];
        }

        //Имя узла
        protected override string NodeType { get { return "Fun"; }}

        //Функция
        public FunCompile Fun { get; private set; }
        //Перегрузка
        public FunOverload Overload { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            var res = Fun.DefineOverload(Args.Select(x => x.Type.DataType).ToArray(), 0, 
                                                        Args.Select(x => x.Type.Simple.ArrayType).ToArray());
            if (res != null)
            {
                Overload = res.Overload;
                Type = new SimpleType(res.DataType, res.ArrayType);    
            }
            else AddError("Недопустимые типы аргументов функции");
        }

        //Запись в скомпилированное выражение
        public override string CompiledText()
        {
            return Overload.Code;
        }

        public override string ToTestString()
        {
            return ToTestWithChildren(Args);
        }
    }
}