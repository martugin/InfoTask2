using System;
using BaseLibrary;
using CommonTypes;
using Different = BaseLibrary.Different;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase
    {
        //Ссылка на соединение
        internal SourceConnect SourceConnect
        {
            get { return (SourceConnect)ProviderConnect; }
        }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Different.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        internal protected TimeInterval GetTime()
        {
            try
            {
                AddEvent("Определение диапазона источника");
                if (!Connect()) return TimeInterval.CreateDefault();
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
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }

        //Подготовка источника
        internal void Prepare()
        {
            try
            {
                using (Start())
                {
                    ClearObjects();
                    foreach (var sig in SourceConnect.InitialSignals.Values)
                    {
                        var ob = AddObject(sig);
                        ob.Context = sig.CodeObject;
                        ob.AddSignal(sig);
                    }
                    Procent = 30;
                    Danger(PrepareSource, 2, 0, "Ошибка при подготовке источника", Reconnect);
                    if (ErrPool == null)
                        ErrPool = new ErrMomPool(MakeErrFactory());
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
            }
        }

        //Очистка списков объектов
        protected abstract void ClearObjects();
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceObject AddObject(InitialSignal sig);
        //Подготовка источника
        protected virtual void PrepareSource() {}
        
        //Создание фабрики ошибок
        protected virtual IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        internal ErrMomPool ErrPool { get; private set; }

        //Создание ошибки 
        internal ErrMom MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Чтение среза, возврашает количество прочитанных значений
        internal protected virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        internal protected abstract ValuesCount ReadChanges();
    }
}