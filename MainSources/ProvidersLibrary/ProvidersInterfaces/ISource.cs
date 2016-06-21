using System;
using BaseLibrary;

namespace CommonTypes
{
    //����������� ��������� ��� ���������� ���������� 
    public interface ISourceConnect : IProviderConnect
    {
        //��������� ��������� ������� ������, ���� �� ������� ���������� - ���������� null
        TimeInterval GetTime();
    }

    //------------------------------------------------------------------------------------------------------------------------

    //����������� ��������� ��� ���������� � ����������
    public interface IProviderSource : IProvider
    {
        //������ ���� �������� ���������, ������� ����� ������ ������
        IDicSForRead<SourceSignal> Signals { get; }
        //������ �������� �� ������
        void GetValues(DateTime beginRead, //������ �������
                               DateTime endRead); //����� �������
        //�������� ������ 
        ErrMom MakeError(int number,  //����� ������
                                      IContextable addr); //�������� ������ (������)
    }

    //------------------------------------------------------------------------------------------------------------------------

    //����������� ��������� ��� ������ Source (�������� �������� ������)
    public interface ISource : IProviderSource
    {
        //�������� ������ � ������ ��������, ��������� ���������� �� ������� ���������� ��� ������
        //���� ������ ��� ����, �� ��� �������� � ���������� � ������ �� ���������
        ISourceSignal AddSignal(string code, //��� �������
                                string context, //�������� �������� (��� ����������� ������� ���������)
                                DataType dataType, //��� ������
                                string signalInf, //���������� � ������� ��� ���������
                                string formula = null); //��������� ������� ���� ����
        //������� �� ��������� ��� �������
        void ClearSignals();

        //������� ��������� ������ ��� ����� � �������, ������ ������ 60
        int CloneCutFrequency { get; }
        //������� �������� � ����
        void MakeClone(DateTime beginRead, //������ ������� �����
                                DateTime endRead, //����� ������� �����
                                string cloneFile); //���� �������� �����
    }

    //------------------------------------------------------------------------------------------------------------------------

    //��������� ������� ���������
    public interface ISourceSignal : IContextable
    {
        //������ ��������
        IMomListReadOnly MomList { get; }
        //���
        string Code { get; }
        //��� ������
        DataType DataType { get; }
    }
}