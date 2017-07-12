using System.Linq;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Узел, задающий метод (получение подпараметра)
    internal class MetNode : TablikKeeperNode
    {
        public MetNode(TablikKeeper keeper, ITerminalNode terminal, IExprNode parent, params IExprNode[] args)
            : base(keeper, terminal, parent)
        {
            Parent = parent;
            IsMet = true;
        }

        //Тип узла
        protected override string NodeType { get { return "Met"; } }

        //Выражение, от которого берется метод
        public IExprNode Parent { get; private set; }

        //Определение типа данных
        public override void DefineType()
        {
            if (!(Parent.Type is TablikParam))
                AddError("Взятие подпараметра от выражения, не являющего параметром");
            else
            {
                var type = Parent.Type;
                while (type is TablikParam)
                {
                    var par = (TablikParam) type;
                    if (par.Params.ContainsKey(Token.Text))
                    {
                        Type = par.Params[Token.Text];
                        break;
                    }
                    type = par.Type;
                }
                if (Type == null)
                    AddError("Не найден подпараметр");
                else
                {
                    var inputs = ((TablikParam)Type).InputsList;
                    if (Args.Length > inputs.Count ||
                        Args.Length < inputs.Count && inputs[Args.Length].DefaultValue == null)
                        AddError("Количество входов подпараметра не совпадает с количеством аргументов");
                    if (Args.Where((t, i) => !t.Type.LessOrEquals(inputs[i].Type)).Any())
                        AddError("Недопустимые типы аргументов подпараметра");
                }
            }
        }
    }
}