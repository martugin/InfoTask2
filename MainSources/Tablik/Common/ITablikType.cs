using CommonTypes;

namespace Tablik
{
    //Интерфейс для типов данных элементов выражения
    internal interface ITablikType
    {
        //Тип данных
        DataType DataType { get; }
        //Тип данных - простой
        SimpleType Simple { get; }
        //Тип данных - сигнал
        ITablikSignalType TablikSignalType { get; }

        //Запись в строку
        string ToResString();
        //Является типом
        bool LessOrEquals(ITablikType type);
    }
}