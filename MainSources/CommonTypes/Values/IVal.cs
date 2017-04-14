using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для всех значений кроме Tabl, Grafic и т.п.
    public interface IVal : ICloneable
    {
        //Значение само, или значение переменной
        Val Value { get; }
        //Расчетное значение 
        ICalcVal CalcValue { get; }
        //Тип данных
        DataType DataType { get; }
    }

    //------------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений, сегментов и массивов
    public interface ICalcVal : IVal
    {
        //Итоговая ошибка для записи в Result
        MomErr TotalError { get; }
    }
    
    //-----------------------------------------------------------------------------------------------
    //Интерфейс для мгновенных значений и списков без учета времени
    public interface IMean : ICalcVal
    {
        //Количество значений
        int Count { get; }
        //Само отдельное значение или последнее значение списка
        IMean LastMom { get; }
        
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }

        //Значения разных типов i-ого значения
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        object ObjectI(int i);

        //Ошибка
        MomErr Error { get; }
        //Ошибка i-ого значения
        MomErr ErrorI(int i);

        //Время или MinDate
        DateTime Time { get; }
        //Время i-ого значения
        DateTime TimeI(int i);

        //Номер текущего значения
        int CurNum { get; set; }
        //Время следующего значения в списке или MaxDate
        DateTime NextTime { get; }
        
        //Сравнение значений и ошибок
        bool ValueEquals(IMean mean);
        bool ValueLess(IMean mean);
        bool ValueAndErrorEquals(IMean mean);

        //Запись значения в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
        //Запись значения в рекордсет rec, поле field
        void ValueToRecI(int i, IRecordAdd rec, string field);

        //Копия значения, возможно с новым временем и ошибкой
        IMean ToMean();
        IMean ToMean(MomErr err);
        IMean ToMom();
        IMean ToMom(MomErr err);
        IMean ToMom(DateTime time);
        IMean ToMom(DateTime time, MomErr err);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IMean ToMeanI(int i);
        IMean ToMeanI(int i, MomErr err);
        IMean ToMomI(int i);
        IMean ToMomI(int i, MomErr err);
        IMean ToMomI(int i, DateTime time);
        IMean ToMomI(int i, DateTime time, MomErr err);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений 
    public interface IMeanList : IMean
    {
        //Добавление мгновенного значения
        void AddMom(IMean mom);
        void AddMom(DateTime time, IMean mean);
        void AddMom(IMean mom, MomErr err);
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