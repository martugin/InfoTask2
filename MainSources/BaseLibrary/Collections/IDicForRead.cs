using System.Collections.Generic;

namespace BaseLibrary
{
    //Интерфейс словаря, в который нельзя добавлять элементы
    public interface IDicIForRead<T>
    {
        //Получение элемента
        T Get(int s);
        //Получение элемента по индексу
        T this[int s] { get; }
        //Содержит элемент
        bool ContainsKey(int s);
        //Содержит значение
        bool ContainsValue(T v);
        //Количество элементов
        int Count { get; }
        //Коллекция ключей словаря
        ICollection<int> Keys { get; }
        //Коллекция значений словаря
        ICollection<T> Values { get; }
    }

    //-----------------------------------------------------------------------------------------------------
    //Интерфейс словаря, в который нельзя добавлять элементы
    public interface IDicSForRead<T>
    {
        //Получение элемента по индексу
        T this[string s] { get; }
        //Содержит элемент
        bool ContainsKey(string s);
        //Количество элементов
        int Count { get; }
        //Коллекция ключей словаря
        ICollection<string> Keys { get; }
        //Коллекция значений словаря
        ICollection<T> Values { get; }
    }
}