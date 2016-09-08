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
    //Интерфейс для мгновенных значений и списков
    public interface ISingleVal : ICalcVal
    {
        //Количество значений
        int Count { get; }
        //Само отделное значение или последнее значение списка
        IMean LastMean { get; }
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений без времени
    public interface IMeanList : ISingleVal
    {
        //Ошибка i-ого значения
        ErrMom ErrorI(int i);

        //Значения разных типов i-ого значения
        bool BooleanI(int i);
        int IntegerI(int i);
        double RealI(int i);
        DateTime DateI(int i);
        string StringI(int i);
        //Запись значения в рекордсет rec, поле field
        void ValueToRecI(IRecordAdd rec, string field, int i);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IMean CloneMeanI(int i);
        IMean CloneMeanI(int i, ErrMom err);
        IMom CloneMomI(int i, DateTime time);
        IMom CloneMomI(int i, DateTime time, ErrMom err);
        //Загрузить буферное значение из списка 
        IMean MeanI(int i);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений только для чтения
    public interface IMomListRead : IMeanList
    {
        //Время i-ого значения
        DateTime TimeI(int i);
        
        IMom CloneMomI(int i);
        IMom CloneMomI(int i, ErrMom err);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для Mean
    public interface IMean : IMeanList
    {
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }

        //Ошибка
        ErrMom Error { get; }

        //Сравнение значений и ошибок
        bool ValueEquals(IMean mean);
        bool ValueLess(IMean mean);
        bool ValueAndErrorEquals(IMean mean);

        //Запись значения в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
        //Копия значения, возможно с новым временем и ошибкой
        IMean CloneMean();
        IMean CloneMean(ErrMom err);
        IMom CloneMom(DateTime time);
        IMom CloneMom(DateTime time, ErrMom err);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для Mom и MomErr
    public interface IMom : IMean, IMomListRead
    {
        //Время
        DateTime Time { get; }
        //Клонирование значения, возможно с добавлением ошибки
        IMom CloneMom();
        IMom CloneMom(ErrMom err);
    }
    
    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений 
    public interface IMomList : IMomListRead
    {
        //Добавление мгновенного значения
        void AddMom(IMom mom);
        void AddMom(DateTime time, IMean mean);
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