namespace Tablik
{
    //Интерфес для сигналов, объектов и типов объектов для Tablik
    internal interface ITablikSignalType : ITablikType
    {
        //Код
        string Code { get; }
        //Имя
        string Name { get; }
    }
}