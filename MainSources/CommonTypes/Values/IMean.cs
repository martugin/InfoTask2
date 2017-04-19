using System;
using BaseLibrary;

namespace CommonTypes
{
    public interface IBaseMean : ICalcVal
    {
        //���������� ��������
        int Count { get; }

        //����� �������� ��������
        int CurNum { get; set; }
        //����� ���������� �������� � ������ ��� MaxDate
        DateTime NextTime { get; }

        //���� ��������� �������� ��� ��������� �������� ������
        IReadMean LastMom { get; }

        //�������� ������ ����� i-��� ��������
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        object ObjectI(int i);

        //������ i-��� ��������
        MomErr ErrorI(int i);
        //����� i-��� ��������
        DateTime TimeI(int i);

        //��������� �������� � ������
        bool ValueEquals(IBaseMean mean);
        bool ValueLess(IBaseMean mean);
        bool ValueAndErrorEquals(IBaseMean mean);

        //������ �������� � ���������
        void ValueToRec(IRecordAdd rec, string field);
        //������ �������� � ��������� rec, ���� field
        void ValueToRecI(int i, IRecordAdd rec, string field);

        //����� ��������, �������� � ����� �������� � �������
        IReadMean ToMean();
        IReadMean ToMom();
        IReadMean ToMom(MomErr err);
        IReadMean ToMom(DateTime time);
        IReadMean ToMom(DateTime time, MomErr err);

        //����� �������� �� �������, �������� � ����� �������� � �������
        IReadMean ToMeanI(int i);
        IReadMean ToMomI(int i);
        IReadMean ToMomI(int i, MomErr err);
        IReadMean ToMomI(int i, DateTime time);
        IReadMean ToMomI(int i, DateTime time, MomErr err);
    }

    //-------------------------------------------------------------------------------------------------
    //��������� ��� ���������� �������� � ������� �������� ������ ��� ������
    public interface IReadMean : IBaseMean
    {
        //�������� ������ �����
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }

        //������
        MomErr Error { get; }
        //����� ��� MinDate
        DateTime Time { get; }
    }

    //-------------------------------------------------------------------------------------------------
    //��������� ��� ���������� �������� � ������� ��������
    public interface IMean : IBaseMean
    {
        //�������� ������ �����
        bool Boolean { get; set; }
        int Integer { get; set; }
        double Real { get; set; }
        DateTime Date { get; set; }
        string String { get; set; }
        object Object { get; set; }

        //������
        MomErr Error { get; set; }
        //����� ��� MinDate
        DateTime Time { get; set; }

        //������ �������� � ���������
        void ValueFromRec(IRecordRead rec, string field);

        //���������� ����������� ��������
        void AddMom(IReadMean mom);
        void AddMom(DateTime time, IReadMean mean, MomErr err);
        //���������� � ��������� ������� � ��������
        void AddMom(DateTime time, bool b, MomErr err = null);
        void AddMom(DateTime time, int i, MomErr err = null);
        void AddMom(DateTime time, double r, MomErr err = null);
        void AddMom(DateTime time, DateTime d, MomErr err = null);
        void AddMom(DateTime time, string s, MomErr err = null);
        void AddMom(DateTime time, object ob, MomErr err = null);

        //�������� ������ ��������
        void Clear();
    }
}
