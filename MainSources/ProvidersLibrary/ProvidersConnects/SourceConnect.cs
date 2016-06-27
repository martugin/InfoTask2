using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Настройки одного подключения источника
    public abstract class SourceConnect : ProviderConnect
    {
        //Получение диапазона времени источника
        internal protected TimeInterval GetTime()
        {
            //Todo переделать на Danger
            try
            {
                return GetSourceTime();
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения временного диапазона источника", ex);
                return new TimeInterval(Different.MinDate, DateTime.Now);
            }
        }
        //Получение времени источника,
        protected virtual TimeInterval GetSourceTime()
        {
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }
    }
}