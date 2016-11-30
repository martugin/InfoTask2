using System.Collections.Generic;
using CommonTypes;

namespace Generator
{
    //Интерфейс узлов обращения к таблицам и запросов для GenRule
    internal interface INodeR : INode
    {
        //Проверка выражения, возвращает уровень таблицы, ряды которого перебираются
        ITablStruct Check(TablsList dataTabls, //Список всех таблиц
                                     ITablStruct parentStruct); //Структрура родительской таблицы
    }

    //-------------------------------------------------------------------------------------------------------------
    //Интерфейс узлов обращения с ктаблицам для GenRule
    interface INodeRTabl : INodeR
    {
        //Сгененирировать таблицу по исходным данным
        IEnumerable<SubRows> SelectRows(TablsList dataTabls, //Список всех таблиц
                                                             SubRows parentRow); //Ряд старшей таблицы, на основе которого производится итерация
        //Следующий узел в цепочке
        INodeRQuery ChildNode { get; set; }
    }

    //-------------------------------------------------------------------------------------------------------------
    //Интерфейс узлов запросов для GenRule
    interface INodeRQuery : INodeR
    {
         //Сгененирировать таблицу по исходным данным
        IEnumerable<SubRows> SelectRows(IEnumerable<SubRows> parentRows); //Ряды предыдущей таблицы, на основе которых производится итерация
    }
}