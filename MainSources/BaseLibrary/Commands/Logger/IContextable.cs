namespace BaseLibrary
{
    //Интерфейс для задания контекста, контекст выводится в сообщениях и т.п.
    public interface IContextable
    {
        string Context { get; }
    }
}