using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    public class InputsVisitor : InputsBaseVisitor<Node>
    {
        public InputsVisitor(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        //Формирование строки ошибки
        private readonly ParsingKeeper _keeper;

        //Обход дерева разбора
        public Node Go(IParseTree tree)
        {
            if (tree == null) return null;
            return Visit(tree);
        }

        public ListNode GoList(IParseTree tree)
        {
            return (ListNode)Go(tree);
        }

        //Обход разных типов узлов


    }
}