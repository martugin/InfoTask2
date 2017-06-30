using System.Collections.Generic;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    internal class FunNode : KeeperNode, IExprNode
    {
        public FunNode(TablikKeeper keeper, ITerminalNode terminal, params ITablikType[] args)
            : base(keeper, terminal)
        {
            Fun = Keeper.App.Funs[Token.Text];
            _args = args;
        }

        //Имя узла
        protected override string NodeType { get { return "Fun"; }}

        //Функция
        public FunClass Fun { get; private set; }
        //Перегрузка
        public FunOverload Overload { get; private set; }

        //Входные значения
        private readonly ITablikType[] _args;

        //Возвращаемый тип
        public ITablikType Type { get; private set; }
        
        //Определение типа данных
        public virtual ITablikType DefineType()
        {
            var res = Fun.DefineOverload(_args);
            Overload = res.Item1;
            return  Type = res.Item2;
        }

        //Запись в скомпилированное выражение
        public virtual string CompiledText()
        {
            return "Fun!" + Overload.Code + "!" + _args.Count;
        }
    }
}