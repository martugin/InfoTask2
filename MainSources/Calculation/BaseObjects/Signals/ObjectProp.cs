using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Одно свойство объекта
    public class ObjectProp
    {
        public ObjectProp(string code, DataType dataType, Mean mean)
        {
            Code = code;
            DataType = dataType;
            Mean = mean;
        }

        //Код
        public string Code { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Значение
        public Mean Mean { get; private set; }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, int objectId)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("CodeProp", Code);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("Mean", Mean.String);
            rec.Update();
        }
    }
}