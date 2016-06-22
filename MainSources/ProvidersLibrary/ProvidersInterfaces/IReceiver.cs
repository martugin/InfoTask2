using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //����������� ��������� ��� ������ Receiver
    public interface IReceiver : IProvider
    {
        //������ �������� ��������� � ����������� ����������
        IDicSForRead<ReceiverSignal> Signals { get; }
        //�������� ������ ���������
        ReceiverSignal AddSignal(string signalInf, //���������� � ������� ��� ���������
                                               string code, //��� �������
                                               DataType dataType); //��� ������
        //��������� �������� ���������� �� ��������
        void WriteValues();
        //����������� �������� ������ ���������� �������� �� ���� ���
        bool AllowListValues { get; }
    }
}