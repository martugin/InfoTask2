using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников чтения из архива
    public abstract class ListSource : Source
    {
        //Ссылка на соединение
        internal ListSourceConnect SourceConnect
        {
            get { return (ListSourceConnect)ProviderConnect; }
        }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Static.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        protected internal TimeInterval GetTime()
        {
            try
            {
                if (!Connect()) return TimeInterval.CreateDefault();
                AddEvent("Определение диапазона источника");
                var ti = GetTimeSource();
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
        //Получение времени источника 
        protected virtual TimeInterval GetTimeSource()
        {
            return new TimeInterval(Static.MinDate, DateTime.Now);
        }
        
        //Чтение среза, возврашает количество прочитанных значений
        protected internal virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        protected internal abstract ValuesCount ReadChanges();

        //Конец предыдущего периода чтения значений
        internal DateTime PrevPeriodEnd { get; set; }
    }
}