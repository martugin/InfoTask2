using System;
using BaseLibrary;

namespace CommonTypes
{
    //��������� ��� ���������� �������� � ������� �������� ������ ��� ������
    public interface IMean : ICalcVal
    {
        //���������� ��������
        int Count { get; }

        //����� �������� ��������
        int CurNum { get; set; }
        //����� ���������� �������� � ������ ��� MaxDate
        DateTime NextTime { get; }

        //���� ��������� �������� ��� ��������� �������� ������
        IMean LastMom { get; }

        //�������� ������ �����
        bool Boolean { get; set; }
        int Integer { get; set; }
        double Real { get; set; }
        DateTime Date { get; set; }
        string String { get; set; }
        object Object { get; set; }

        //�������� ������ ����� i-��� ��������
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        object ObjectI(int i);

        //������
        MomErr Error { get; set; }
        //������ i-��� ��������
        MomErr ErrorI(int i);

        //����� ��� MinDate
        DateTime Time { get; set; }
        //����� i-��� ��������
        DateTime TimeI(int i);

        //��������� �������� � ������
        bool ValueEquals(IMean mean);
        bool ValueLess(IMean mean);
        bool ValueAndErrorEquals(IMean mean);

        //������ �������� � ���������
        void ValueToRec(IRecordAdd rec, string field);
        //������ �������� � ��������� rec, ���� field
        void ValueToRecI(int i, IRecordAdd rec, string field);
        //������ �������� � ���������
        void ValueFromRec(IRecordRead rec, string field);

        //����� ��������, �������� � ����� �������� � �������
        IMean ToMean();
        IMean ToMom();
        IMean ToMom(MomErr err);
        IMean ToMom(DateTime time);
        IMean ToMom(DateTime time, MomErr err);

        //����� �������� �� �������, �������� � ����� �������� � �������
        IMean ToMeanI(int i);
        IMean ToMomI(int i);
        IMean ToMomI(int i, MomErr err);
        IMean ToMomI(int i, DateTime time);
        IMean ToMomI(int i, DateTime time, MomErr err);

        //���������� ����������� ��������
        void AddMom(IMean mom);
        void AddMom(DateTime time, IMean mean, MomErr err);
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
