using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Generator
{
    //Узел - вызывающий итерацию по строкам таблицы, базовый для Tabl, SubTabl, GroupTabl
    internal abstract class NodeRTablBase : NodeKeeper
    {
        protected NodeRTablBase(ParsingKeeper keeper, ITerminalNode terminal, NodeRTablBase childNode = null) 
            : base(keeper, terminal)
        {
            ChildNode = childNode;
        }

        //Проверка выражения, возвращает уровень таблицы, ряды которого перебираются
        public abstract IRowStruct Check(TablsList dataTabls, //Список всех таблиц
                                                           TablStruct parentStruct); //Структрура родительской таблицы
        //Сгененирировать таблицу по исходным данным
        public abstract IEnumerable<SubRows> SelectRows(TablsList dataTabls, //Список всех таблиц
                                                        IEnumerable<SubRows> parentRows); //Ряды предыдущей таблицы, на основе которых производится итерация
        //Следующий узел в цепочке
        public NodeRTablBase ChildNode { get; private set; }
    }
}