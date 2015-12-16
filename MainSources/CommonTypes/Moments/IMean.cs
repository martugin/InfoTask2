using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для Mean
    public interface IMean : IMomentsVal
    {
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }

        //Сравнение значений и ошибок
        bool ValueEquals(IMean mean);
        bool ValueLess(IMean mean);
        bool ValueAndErrorEquals(IMean mean);

        //Запись и чтение значения в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
        //Копия значения с новым временем
        IMom Clone(DateTime time, ErrMom err = null);
    }
    
    //----------------------------------------------------------------------------------------------------------------------------
    //Интерфейс для Mom и MomEdit
    public interface IMom : IMean
    {
        //Время
        DateTime Time { get; }
    } 
}