using BaseLibrary;

namespace Calculation
{
    //������� ����� ��� �������� ������� � ����������
    public abstract class BaseObject
    {
        protected BaseObject(IRecordRead rec)
        {
            Id = rec.GetInt("ObjectId");
            Code = rec.GetString("CodeObject");
            Name = rec.GetString("NameObject");
            InfObject = rec.GetString("InfObject");
        }

        //Id
        public int Id { get; private set; }
        //��� 
        public string Code { get; private set; }
        //���
        public string Name { get; private set; }
        //�������� ��� ���������
        public string InfObject { get; private set; }

        //������ �������� � �� ���� ������
        private readonly DicS<ObjectProp> _props = new DicS<ObjectProp>();
        public DicS<ObjectProp> Props { get { return _props; } }
    }
}