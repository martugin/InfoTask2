using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Настройки одного подключения источника
    public abstract class SourceConnect : ProviderConnect
    {
        //Получение диапазона времени источника
        //Возвращает TimeInterval(Different.MinDate, DateTime.MaxValue) если нет связи с источником
        //Возвращает TimeInterval(Different.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        internal protected TimeInterval GetTime()
        {
            try
            {
                AddEvent("Определение диапазона источника");
                var ti =  GetSourceTime();
                if (!ti.IsDefault)
                    AddEvent("Диапазон источника определен", ti.Begin + " - " + ti.End);
                else AddError("Диапазон источника не определен");
                return ti;
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения временного диапазона источника", ex);
                return TimeInterval.CreateDefault();
            }
        }
        //Получение времени источника,
        protected virtual TimeInterval GetSourceTime()
        {
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }
    }
}