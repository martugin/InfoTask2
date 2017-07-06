using CommonTypes;

namespace CompileLibrary
{
    //���� �������� �������
    internal class FunParam
    {
        internal FunParam(string dtype, string atype = "", string def = "")
        {
            DataType = dtype.ToDataType();
            if (DataType == DataType.Error)
                DataType = DataType.Variant;
            ArrayType = atype.ToArrayType();
            Default = def;
        }

        //��� ������, ���������� ��� ��� ������� ��� ������ �������
        internal DataType DataType { get; private set; }
        //��� �������
        internal ArrayType ArrayType { get; private set; }
        //�������� �� ��������� ��� null
        internal string Default { get; private set; }
        //True, ���� ��������� � ������������ ���� ����������
        internal bool UsedInResult { get; set; }
    }
}