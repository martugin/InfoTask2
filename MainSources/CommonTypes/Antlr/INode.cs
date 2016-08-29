using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Интерфейс для всех узлов ANTLR
    public interface INode
    {
        //Ссылка на токен
        IToken Token { get; }
        //Запись в строку для тестов
        string ToTestString();
    }

    //-------------------------------------------------------------------------------------------------
    //Интерфейс для узлов ANTLR, возвращающих текст
    public interface INodeText : INode
    {
        //Возвращаемый текст
        string GetText();
    }
}