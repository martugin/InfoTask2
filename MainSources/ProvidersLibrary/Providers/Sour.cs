using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для провайдеров источников 
    public abstract class Sour : Prov
    {
        protected Sour()
        {
            NeedCut = true;
        }

        //Ссылка на соединение
        public SourConn SourceConn { get { return (SourConn)Conn; } }

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
                AddError("Ошибка определения временного диапазона источника", ex, CodeObject);
                return new TimeInterval(Different.MinDate, DateTime.Now);
            }
        }
        //Получение времени источника,
        protected virtual TimeInterval GetSourceTime()
        {
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }

        //Чтение значений из источника
        internal void GetValues()
        {
            if (NeedCut)
                Start(ReadCut, 0, SourceConn.PeriodBegin < SourceConn.PeriodEnd ? 30 : 100);
            if (!NeedCut || SourceConn.PeriodBegin < SourceConn.PeriodEnd)
                Start(ReadChanges, Procent, 100);
        }
        
        //Нужно считывать срез
        protected bool NeedCut { get; set; }
        //Чтение среза
        protected virtual void ReadCut() { }
        //Чтение изменений
        protected virtual void ReadChanges() { }
    }
}