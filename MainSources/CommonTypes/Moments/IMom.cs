using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для Mean
    public interface IMean : ICalcVal
    {
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
        object Object { get; }
    }
    
    //----------------------------------------------------------------------------------------------------------------------------
    //Интерфейс для Mom и MomEdit
    public interface IMom : IMomentsVal, IMean
    {
        //Время
        DateTime Time { get; }
        
        //Сравнение значений и ошибок
        bool ValueEquals(IMom mom);
        bool ValueLess(IMom mom);
        bool ValueAndErrorEquals(IMom mom);

        //Запись в рекордсет
        void ValueToRec(IRecordAdd rec, string field);
    } 
}