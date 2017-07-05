using Antlr4.Runtime;

namespace CompileLibrary
{
    //Интерфейс для всех узлов ANTLR
    public interface INode
    {
        //Ссылка на токен
        IToken Token { get; }
        //Запись в строку для тестов
        string ToTestString();
    }
}