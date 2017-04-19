using System;
using BaseLibrary;

namespace CommonTypes
{
    public interface IBaseMean : ICalcVal
    {
        //Количество значений
        int Count { get; }

        //Номер текущего значения
        int CurNum { get; set; }
        //Время следующего значения в списке или MaxDate
        DateTime NextTime { get; }

        //Само отдельное значение или последнее значение списка
        IReadMean LastMom { get; }

        //Значения разных типов i-ого значения
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        object ObjectI(int i);

        //Ошибка i-ого значения
        MomErr ErrorI(int i);
        //Время i-ого значения
        DateTime TimeI(int i);

        //Сравнение значений и ошибок
        bool ValueEquals(IBaseMean mean);
        bool ValueLess(IBaseMean mean);
        bool ValueAndErrorEquals(IBaseMean mean);

        //Запись значения в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
        //Запись значения в рекордсет rec, поле field
        void ValueToRecI(int i, IRecordAdd rec, string field);

        //Копия значения, возможно с новым временем и ошибкой
        IReadMean ToMean();
        IReadMean ToMom();
        IReadMean ToMom(MomErr err);
        IReadMean ToMom(DateTime time);
        IReadMean ToMom(DateTime time, MomErr err);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IReadMean ToMeanI(int i);
        IReadMean ToMomI(int i);
        IReadMean ToMomI(int i, MomErr err);
        IReadMean ToMomI(int i, DateTime time);
        IReadMean ToMomI(int i, DateTime time, MomErr err);
    }

    //-------------------------------------------------------------------------------------------------
    //Интерфейс для мгновенных значений и списков значений только для чтения
    public interface IReadMean : IBaseMean
    {
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }

        //Ошибка
        MomErr Error { get; }
        //Время или MinDate
        DateTime Time { get; }
    }

    //-------------------------------------------------------------------------------------------------
    //Интерфейс для мгновенных значений и списков значений
    public interface IMean : IBaseMean
    {
        //Значения разных типов
        bool Boolean { get; set; }
        int Integer { get; set; }
        double Real { get; set; }
        DateTime Date { get; set; }
        string String { get; set; }
        object Object { get; set; }

        //Ошибка
        MomErr Error { get; set; }
        //Время или MinDate
        DateTime Time { get; set; }

        //Запись значения в рекордсет
        void ValueFromRec(IRecordRead rec, string field);

        //Добавление мгновенного значения
        void AddMom(IReadMean mom);
        void AddMom(DateTime time, IReadMean mean, MomErr err);
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
