using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для мгновенных значений и списков значений только для чтения
    public interface IMean : ICalcVal
    {
        //Количество значений
        int Count { get; }

        //Номер текущего значения
        int CurNum { get; set; }
        //Время следующего значения в списке или MaxDate
        DateTime NextTime { get; }

        //Само отдельное значение или последнее значение списка
        IMean LastMom { get; }

        //Значения разных типов
        bool Boolean { get; set; }
        int Integer { get; set; }
        double Real { get; set; }
        DateTime Date { get; set; }
        string String { get; set; }
        object Object { get; set; }

        //Значения разных типов i-ого значения
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        object ObjectI(int i);

        //Ошибка
        MomErr Error { get; set; }
        //Ошибка i-ого значения
        MomErr ErrorI(int i);

        //Время или MinDate
        DateTime Time { get; set; }
        //Время i-ого значения
        DateTime TimeI(int i);

        //Сравнение значений и ошибок
        bool ValueEquals(IMean mean);
        bool ValueLess(IMean mean);
        bool ValueAndErrorEquals(IMean mean);

        //Запись значения в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
        //Запись значения в рекордсет rec, поле field
        void ValueToRecI(int i, IRecordAdd rec, string field);
        //Запись значения в рекордсет
        void ValueFromRec(IRecordRead rec, string field);

        //Копия значения, возможно с новым временем и ошибкой
        IMean ToMean();
        IMean ToMom();
        IMean ToMom(MomErr err);
        IMean ToMom(DateTime time);
        IMean ToMom(DateTime time, MomErr err);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IMean ToMeanI(int i);
        IMean ToMomI(int i);
        IMean ToMomI(int i, MomErr err);
        IMean ToMomI(int i, DateTime time);
        IMean ToMomI(int i, DateTime time, MomErr err);

        //Добавление мгновенного значения
        void AddMom(IMean mom);
        void AddMom(DateTime time, IMean mean, MomErr err);
        //Добавление с указанием времени и значения
        void AddMom(DateTime time, bool b, MomErr err = null);
        void AddMom(DateTime time, int i, MomErr err = null);
        void AddMom(DateTime time, double r, MomErr err = null);
        void AddMom(DateTime time, DateTime d, MomErr err = null);
        void AddMom(DateTime time, string s, MomErr err = null);
        void AddMom(DateTime time, object ob, MomErr err = null);

        //Очистить список значений
        void Clear();
    }
}
