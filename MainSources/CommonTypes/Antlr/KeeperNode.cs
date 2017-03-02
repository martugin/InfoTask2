using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Узел с возможностью записи ошибок
    public abstract class KeeperNode : Node
    {
        protected KeeperNode(ParsingKeeper keeper, ITerminalNode terminal) : base(terminal)
        {
            Keeper = keeper;
        }

        //Накопитель ошибок
        protected ParsingKeeper Keeper { get; private set; }

        //Добавить ошибку и вернуть пустое значение
        protected void AddError(string text)
        {
            Keeper.AddError(text, Token);
        }
    }
}