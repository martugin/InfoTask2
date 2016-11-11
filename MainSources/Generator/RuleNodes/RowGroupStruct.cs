using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Структура группы рядов
    public class RowGroupStruct
    {
        public RowGroupStruct(TablStruct tablStruct, IEnumerable<string> fields)
        {
            TablStruct = tablStruct;
            Fields = new SetS();
            foreach (var field in fields)
                Fields.Add(field);
        }

        //Ссылка на структуру таблицы
        public TablStruct TablStruct { get; private set; }
        //Множество полей, по которым производится группировка
        public SetS Fields { get; private set; }
    }
}