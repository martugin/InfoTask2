using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Структура группы рядов
    public class RowGroupStruct : IRowStruct
    {
        public RowGroupStruct(TablStruct tablStruct, IEnumerable<string> fields)
        {
            TablStruct = tablStruct;
            Fields = new DicS<DataType>();
            foreach (var field in fields)
                Fields.Add(field, tablStruct.Fields[field]);
        }

        //Ссылка на структуру таблицы
        public TablStruct TablStruct { get; private set; }
        //Множество полей, по которым производится группировка
        public DicS<DataType> Fields { get; private set; }
    }
}