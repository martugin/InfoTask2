namespace Tablik
{
    //Признак обхода в глубину
    public enum DfsStatus
    {
        Before, //Вершина графа не посещена
        Process, //Из вершину графа произведен рекурсивный вызов
        After //Вершина графа посещена
    }
}