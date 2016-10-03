namespace CommonTypes
{
    //���� �������� �������
    internal class FunParam
    {
        internal FunParam(string dtype, string def = null)
        {
            DataType = dtype.ToDataType();
            if (DataType == DataType.Error)
                DataType = DataType.Variant;
            Default = def;
        }

        //��� ������, ���������� ��� ��� ������� ��� ������ �������
        internal DataType DataType { get; private set; }
        //�������� �� ��������� ��� null
        internal string Default { get; private set; }
        //True, ���� ��������� � ������������ ���� ����������
        internal bool UsedInResult { get; set; }
    }
}