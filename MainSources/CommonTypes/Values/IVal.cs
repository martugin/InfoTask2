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
    //Интерфейс для Mean
    public interface IMean : ISingleVal
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
        //Копия значения с новым временем и ошибкой
        IMom Clone(DateTime time);
        IMom Clone(DateTime time, ErrMom err);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для Mom и MomErr
    public interface IMom : IMean
    {
        //Время
        DateTime Time { get; }
        //Клонирование значения
        IMom Clone();
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений только для чтения
    public interface IMomListReadOnly : ISingleVal
    {
        //Время i-ого значения
        DateTime Time(int i);
        //Ошибка i-ого значения
        ErrMom Error(int i);

        //Значения разных типов i-ого значения
        bool Boolean(int i);
        int Integer(int i);
        double Real(int i);
        DateTime Date(int i);
        string String(int i);

        //Копия значения по индексу, возможно с новым временем и ошибкой
        IMom Clone(int i);
        IMom Clone(int i, DateTime time);
        IMom Clone(int i, DateTime time, ErrMom err);
        //Загрузить буферное значение из списка 
        Mean Mom(int i);
    }

    //-----------------------------------------------------------------------------------------------
    //Интерфейс для списков мгновенных значений 
    public interface IMomList : IMomListReadOnly
    {
        //Добавление мгновенного значения
        void AddMom(IMom mom);
        void AddMom(DateTime time, IMean mean);
        //Дгобавление с указанием времени и значения
        void AddMom(DateTime time, bool b, ErrMom err = null);
        void AddMom(DateTime time, int i, ErrMom err = null);
        void AddMom(DateTime time, double r, ErrMom err = null);
        void AddMom(DateTime time, DateTime d, ErrMom err = null);
        void AddMom(DateTime time, string s, ErrMom err = null);

        //Очистить список значений
        void Clear();
    }
}