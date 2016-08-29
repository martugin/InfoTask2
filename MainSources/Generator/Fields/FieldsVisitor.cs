using Antlr4.Runtime.Tree;
using CommonTypes;
using Generator.Fields;

namespace Generator
{
    internal class FieldsVisitor : FieldsParsemesBaseVisitor<Node> 
    {
        public FieldsVisitor(ParsingKeeper keeper)
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

        //Обход разных типов узлов


    }
}