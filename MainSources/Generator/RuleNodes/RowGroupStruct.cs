using System.Collections.Generic;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Generator
{
    //Структура группы рядов
    public class RowGroupStruct : ITablStruct
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

        //Следующий и предыдущий уровни таблицы
        public TablStruct Child { get { return TablStruct; } }
        public TablStruct Parent { get { return TablStruct.Parent; } }

        //Имя таблицы
        public string TableName { get { return TablStruct.TableName; } }
        //Уровень таблицы в группе
        public int Level { get { return TablStruct.Level; } }
    }
}