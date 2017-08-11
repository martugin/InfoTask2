using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Ветвление
    internal class IfNode : TablikKeeperNode
    {
        public IfNode(TablikKeeper keeper, ITerminalNode terminal, IEnumerable<IExprNode> conditions, IEnumerable<IExprNode> variants) 
            : base(keeper, terminal)
        {
            _conditions = conditions.ToList();
            _variants = variants.ToList();
            Args = new IExprNode[_conditions.Count + _variants.Count];
            for (int i = 0; i < _conditions.Count; i++)
            {
                Args[2*i] = _conditions[i];
                Args[2*i+1] = _variants[i];
            }
            if (_variants.Count > _conditions.Count)
                Args[Args.Length-1] = _variants[_variants.Count-1];
        }

        //Тип узла
        protected override string NodeType { get { return "If"; } }

        //Список условий
        private readonly List<IExprNode> _conditions;
        //Список вариантов значений
        private readonly List<IExprNode> _variants;

        //Проверка типов данных
        public override void DefineType()
        {
            foreach (var c in _conditions)
                if (c.Type.DataType != DataType.Boolean)
                {
                    AddError("Недопустимый тип данных условия");
                    break;
                }
            foreach (var expr in _variants)
                Type = Type.Add(expr.Type);
            if (Type.DataType == DataType.Error)
                AddError("Несовместимые типы данных аргументов функции");
        }

        public override string ToTestString()
        {
            return ToTestWithChildren(Args);
        }
    }
}