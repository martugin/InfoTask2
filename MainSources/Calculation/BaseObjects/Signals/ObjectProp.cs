using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //���� �������� �������
    public class ObjectProp
    {
        public ObjectProp(string code, DataType dataType, Mean mean)
        {
            Code = code;
            DataType = dataType;
            Mean = mean;
        }

        //���
        public string Code { get; private set; }
        //��� ������
        public DataType DataType { get; private set; }
        //��������
        public Mean Mean { get; private set; }

        //������ � ���������
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