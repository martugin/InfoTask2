using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для всех значений кроме Tabl, Grafic и т.п.
    public interface IVal
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
        ErrMom TotalError { get; }
    }
    
    //-----------------------------------------------------------------------------------------------
    //Интерфейс для мгновенных значений и списков без учета времени
    public interface IMean : ICalcVal
    {
        //Количество значений
        int Count { get; }
        //Само отдельное значение или последнее значение списка
        IMean LastMean { get; }
        
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
        ErrMom Error { get; }
        //Ошибка i-ого значения
        ErrMom ErrorI(int i);

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
        void ValueToRecI(IRecordAdd rec, string field, int i);

        //Копия значения, возможно с новым временем и ошибкой
        IMean CloneMean();
        IMean CloneMean(ErrMom err);
        IMean CloneMom();
        IMean CloneMom(ErrMom err);
        IMean CloneMom(DateTime time);
        IMean CloneMom(DateTime time, ErrMom err);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IMean CloneMeanI(int i);
        IMean CloneMeanI(int i, ErrMom err);
        IMean CloneMomI(int i);
        IMean CloneMomI(int i, ErrMom err);
        IMean CloneMomI(int i, DateTime time);
        IMean CloneMomI(int i, DateTime time, ErrMom err);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений 
    public interface IMeanList : IMean
    {
        //Добавление мгновенного значения
        void AddMom(IMean mom);
        void AddMom(DateTime time, IMean mean);
        void AddMom(IMean mom, ErrMom err);
        void AddMom(DateTime time, IMean mean, ErrMom err);
        //Добавление с указанием времени и значения
        void AddMom(DateTime time, bool b, ErrMom err = null);
        void AddMom(DateTime time, int i, ErrMom err = null);
        void AddMom(DateTime time, double r, ErrMom err = null);
        void AddMom(DateTime time, DateTime d, ErrMom err = null);
        void AddMom(DateTime time, string s, ErrMom err = null);
        void AddMom(DateTime time, object ob, ErrMom err = null);

        //Очистить список значений
        void Clear();
    }
}